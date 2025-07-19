using Application.DTO_s.CategoryDto_s;
using Application.DTO_s;
using Application.Interfaces;
using Application.ResponseDTO_s.CategoryResponse;
using Application.ResponseDTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO_s.SupplierDto_s;
using Application.ResponseDTO_s.SupplierResponse;
using Domain.Interface;
using Domain.Entity;
using Application.Constants;
using Application.ResponseDTO_s.ProductResponse;
using Application.Mappings;
using Domain.Parameters;
using Application.Constants.Enum;
using System.Data;
using Domain.Enum;
using Microsoft.AspNetCore.Http;
using Application.ResponseDTO_s.PurchaseOrder;

namespace Application.Services
{
	public class SupplierServices : ISupplierServices
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IAuthService authService;
		private readonly RoleBasedSupplierMapper mapper;
		private readonly RoleBasedPurchaseOrderMapper roleBasedPurchaseOrderMapper;
		private readonly IImageStorageService imageStorageService;
		private readonly IWarehouseService warehouseService;
		private readonly IUserContextService userContextService;
		public SupplierServices(IUnitOfWork unitOfWork, IAuthService authService, RoleBasedSupplierMapper mapper
								, IWarehouseService warehouseService, IImageStorageService imageStorageService
								, RoleBasedPurchaseOrderMapper roleBasedPurchaseOrderMapper, IUserContextService userContextService)
		{
			this.unitOfWork = unitOfWork;
			this.authService = authService;
			this.mapper = mapper;
			this.imageStorageService = imageStorageService;
			this.roleBasedPurchaseOrderMapper = roleBasedPurchaseOrderMapper;
			this.warehouseService = warehouseService;
			this.userContextService = userContextService;
		}
		public async Task<ApiResponse<ConfirmationResponseDto>> CreateSupplierAsync(CreateSupplierDto dto)
		{
			// Check if email already exists
			bool emailExists = await authService.IsEmailUniqueAsync(dto.Email);
			if (emailExists)
				return ApiResponse<ConfirmationResponseDto>.ValidationError($"Supplier with email '{dto.Email}' already exists.");

			bool companyNameExists = await unitOfWork.SupplierRepository.IsCompanyNameExistsAsync(dto.CompanyName);
			if (companyNameExists)
				return ApiResponse<ConfirmationResponseDto>.ValidationError($"Supplier with company name '{dto.CompanyName}' already exists.");

			bool userNameExists = await authService.IsUserNameUniqueAsync(dto.UserName);
			if (userNameExists)
				return ApiResponse<ConfirmationResponseDto>.ValidationError("Username '{dto.UserName}' already exists.");

			var userId = await authService.CreateSupplierAsync(dto);
			var supplier = new Supplier
			{
				CompanyName = dto.CompanyName,
				Address = dto.Address,
				CreateOn = DateTime.Now,
				UserId = userId,
			};

			await unitOfWork.BeginTransactionAsync();
			await unitOfWork.SupplierRepository.AddAsync(supplier);
			await unitOfWork.CommitTransaction();

			ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			{
				Message = $"Supplier '{supplier.CompanyName}' with ID {supplier.SupplierId} Created Successfully",
				status = ConfirmationStatus.Created
			};
			return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 201);
		}

		public async Task<ApiResponse<SupplierResponseDto>> GetSupplierByIdAsync(int supplierId, ProductPaginationForSupplierQuery qP)
		{
			var supplier = await unitOfWork.SupplierRepository.GetByIdAsync(supplierId);
			if (supplier is null)
				return ApiResponse<SupplierResponseDto>.Failuer(404, $"Supplier with Id '{supplierId}' Not Found ");
			//return ApiResponse<SupplierResponseDto>.ValidationError($"Category with name '{dto.Name}' already exists.")

			var productFilter = new BaseFilter
			{
				PageSize = qP.PageSize,
				PageNumber = qP.PageNumber,
			};
			var (productCount, productwithpage) = await unitOfWork.ProductRepository.GetProductsBySupplierAsync(supplierId, productFilter);
			supplier.Products = productwithpage;
			qP.TotalCount = productCount;

			var respondDto = mapper.MapSupplierToResponseDto(supplier, qP);

			return ApiResponse<SupplierResponseDto>.Success(respondDto, 200);
		}

		public async Task<PagedResponse<List<SupplierListRespondDto>>> GetPaginatedSuppliersAsync(SupplierQueryParameters qP)
		{
			var Filter = new BaseFilter
			{
				PageNumber = qP.PageNumber,
				PageSize = qP.PageSize,
				searchTearm = qP.searchTearm?.ToLower(),
				SortAscending = qP.SortAscending,
				SortBy = qP.SortOption.ToString(),
			};
			var (totalCount, suppliers) = await unitOfWork.SupplierRepository.SupplierWithProductCountAsync(Filter);

			if (totalCount == 0)
				return PagedResponse<List<SupplierListRespondDto>>.SimpleResponse(null,
					qP.PageNumber
					, qP.PageSize,
					totalCount,
					"No suppliers found matching the search criteria.");

			var supplierDtos = suppliers.Select(e => mapper.MapToSupplierListDto(e)).ToList();

			var pagedResponse = PagedResponse<List<SupplierListRespondDto>>.SimpleResponse(supplierDtos, qP.PageSize, qP.PageSize, totalCount);
			return pagedResponse;
		}
		public async Task<ApiResponse<ConfirmationResponseDto>> ChangeVerificationStatusAsync(ChangeSupplierVerificationStatusDto dto)
		{
			var existingSupplier = await unitOfWork.SupplierRepository.GetByIdAsync(dto.SupplierId);
			if (existingSupplier == null || existingSupplier.IsDeleted)
				return ApiResponse<ConfirmationResponseDto>.Failuer(404, $"Supplier with Id '{existingSupplier.SupplierId}' Not Found ");

			var newStatus = (Domain.Enum.VerificationStatus)dto.newStatus;

			if (existingSupplier.VerificationStatus == newStatus)
				return ApiResponse<ConfirmationResponseDto>.Success(null, 200, "Supplier already has this verification status.");

			existingSupplier.VerificationStatus = newStatus;
			existingSupplier.LastUpdateOn = DateTime.Now;

			if (dto.newStatus == Application.Constants.Enum.VerificationStatus.Verified)
				existingSupplier.IsVerified = true;
			if (dto.newStatus == Application.Constants.Enum.VerificationStatus.Rejected)
			{
				existingSupplier.IsVerified = false;
				existingSupplier.RejectionReason = dto.RejectionReason;
			}

			await unitOfWork.BeginTransactionAsync();
			await unitOfWork.CommitTransaction();

			ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			{
				Message = $"Supplier '{existingSupplier.CompanyName}' with ID {existingSupplier.SupplierId} has been {existingSupplier.VerificationStatus.ToString()} successfully",
				status = ConfirmationStatus.Updated
			};
			return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 200);
		}

		public async Task<ApiResponse<SupplierVerificationStatusBaseRespondDto>> GetVerificationStatusByIdAsync(string supplierId)
		{
			Supplier existingSupplier;

			if (userContextService.IsSupplier)
				existingSupplier = await unitOfWork.SupplierRepository.GetSupplierByUserIdAsync(supplierId);
			else
				existingSupplier = await unitOfWork.SupplierRepository.GetByIdAsync(int.Parse(supplierId));
			
			if (existingSupplier is null)
				return ApiResponse<SupplierVerificationStatusBaseRespondDto>.Failuer(404, $"Supplier with Id '{existingSupplier?.SupplierId}' Not Found ");

			var responseDto = new SupplierVerificationStatusBaseRespondDto
			{
				Status = existingSupplier.VerificationStatus.ToString(),
				Reason = existingSupplier.RejectionReason ?? "No enter reason",
				Verified=existingSupplier.IsVerified
			};
			return ApiResponse<SupplierVerificationStatusBaseRespondDto>.Success(responseDto, 200, "Verification status retrieved successfully");
		}

		public async Task<ApiResponse<List<SupplierVerificationStatusRespondDto>>> GetSuppliersByVerificationStatusAsync(bool? isVerified = null)
		{
			var suppliers = await unitOfWork.SupplierRepository.GetSuppliersByVerificationStatusAsync(isVerified);

			var responseDtos = suppliers.Select(e => e.ToResponseDto()).ToList();
			var message = isVerified switch
			{
				true => "Verified suppliers retrieved successfully",
				false => "Unverified suppliers retrieved successfully",
				null => "All suppliers retrieved successfully"
			};
			return ApiResponse<List<SupplierVerificationStatusRespondDto>>.Success(responseDtos, 200, message);
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> UploadSupplierTaxDocumentAsync(string supplierId, FileUploadDto file)
		{
			var existingSupplier = await unitOfWork.SupplierRepository.GetSupplierByUserIdAsync(supplierId);
			if (existingSupplier == null || existingSupplier.IsDeleted)
				return ApiResponse<ConfirmationResponseDto>.Failuer(404, $"Supplier with Id '{supplierId}' Not Found ");

			if (!string.IsNullOrEmpty(existingSupplier.TaxDocumentPath))
				imageStorageService.DeleteImage(existingSupplier.TaxDocumentPath);

			var (uploadSuccess, fileName) = await imageStorageService.UploadImage(file.ImageFile, "supplier-documents");

			if (!uploadSuccess)
				throw new Exception();
			// throw new InternalServerException("Failed to upload document."); //////////////////////////////////

			existingSupplier.TaxDocumentPath = $"supplier-documents/{fileName}";
			existingSupplier.LastUpdateOn = DateTime.Now;

			await unitOfWork.BeginTransactionAsync();
			await unitOfWork.CommitTransaction();

			ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			{
				Message = $"Tax document uploaded successfully for supplier '{existingSupplier.CompanyName}' with ID {existingSupplier.SupplierId}",
				status = ConfirmationStatus.upload
			};
			return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 200);
		}

		public async Task<ApiResponse<PurchaseOrderDetailsResponseDto>> SimulateSupplierReceivingAsync(int purchaseOrderId)
		{
			var purchaseOrder = await unitOfWork.PurchaseOrderRepository
												.GetPurchaseOrderWithItemsAndSupplierAsync(purchaseOrderId);
			if (purchaseOrder is null)
				return ApiResponse<PurchaseOrderDetailsResponseDto>.Failuer(404, $"Supplier with Id '{purchaseOrderId}' Not Found ");

			var allowedStatuses = new[] { PurchaseOrderStatus.Sent, PurchaseOrderStatus.PartiallyReceived };

			if (!allowedStatuses.Contains(purchaseOrder.PurchaseOrderStatus))
				return ApiResponse<PurchaseOrderDetailsResponseDto>.Success(null, 200
										  , "Purchase order status doesn't allow receiving simulation.");

			Random rnd = new Random();
			foreach (var item in purchaseOrder.OrderItems.Where(e => e.ReceivedQuantity < e.OrderQuantity))
				item.ReceivedQuantity = SimulateItemReceiving(item, rnd);

			await unitOfWork.BeginTransactionAsync();
			UpdatePurchaseOrderStatus(purchaseOrder);
			await unitOfWork.CommitTransaction();

			///////will call it her as background job  SendProductsToWarehouse(PurchaseOrder purchaseOrder)
			await warehouseService.SendProductsToWarehouse(purchaseOrder);

			var response = roleBasedPurchaseOrderMapper.MapToPurchaseOrderDetailsDtoAsync(purchaseOrder);
			return ApiResponse<PurchaseOrderDetailsResponseDto>.Success(response, 200);
		}
		private int SimulateItemReceiving(PurchaseOrderItem item, Random random)
		{
			var remainingQuantity = item.OrderQuantity - item.ReceivedQuantity;
			var chance = random.Next(1, 101);

			return chance switch
			{
				<= 85 => item.OrderQuantity,
				<= 95 => item.ReceivedQuantity + random.Next(1, remainingQuantity + 1),
				_ => item.ReceivedQuantity,
			};
		}
		private void UpdatePurchaseOrderStatus(PurchaseOrder purchaseOrder)
		{
			var isFullyReceived = purchaseOrder.OrderItems.All(e => e.OrderQuantity == e.ReceivedQuantity);

			purchaseOrder.PurchaseOrderStatus = isFullyReceived switch
			{
				true => purchaseOrder.PurchaseOrderStatus = PurchaseOrderStatus.Received,
				false => purchaseOrder.PurchaseOrderStatus = PurchaseOrderStatus.PartiallyReceived,
			};
			purchaseOrder.LastUpdateOn = DateTime.Now;
		}
		public async Task<ApiResponse<ConfirmationResponseDto>> UpdateSupplierAsync(string id, UpdateSupplierDto dto)
		{
			Supplier existingSupplier;

			if (userContextService.IsSupplier)
				existingSupplier = await unitOfWork.SupplierRepository.GetSupplierByUserIdAsync(id);
			else
				existingSupplier = await unitOfWork.SupplierRepository.GetByIdAsync(int.Parse(id));

			if (existingSupplier is null)
				return ApiResponse<ConfirmationResponseDto>.Failuer(404, $"Supplier with Id '{existingSupplier.SupplierId}' Not Found ");

			bool companyNameExists = await unitOfWork.SupplierRepository
				  .IsCompanyNameExistsAsync(dto.CompanyName, existingSupplier.SupplierId);

			if (companyNameExists)
				return ApiResponse<ConfirmationResponseDto>.ValidationError($"Supplier with company name '{dto.CompanyName}' already exists.");

			mapper.MapUpdateDtoToSupplier(dto, existingSupplier);
			await unitOfWork.BeginTransactionAsync();
			await unitOfWork.CommitTransaction();

			ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			{
				Message = $"Supplier '{existingSupplier.CompanyName}' with ID {existingSupplier.SupplierId} Updated Successfully",
				status = ConfirmationStatus.Updated
			};
			return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 200);
		}

		public async Task<ApiResponse<List<PendingPurchaseOrderResponseDto>>> GetPendingPurchaseOrdersForSupplierAsync()
		{
			var supplier = await unitOfWork.SupplierRepository.GetSupplierByUserIdAsync(userContextService.userId);
			if (supplier is null)
				return ApiResponse<List<PendingPurchaseOrderResponseDto>>.Failuer(404, $"Supplier with  Not Found");

			var purchaseOrders = await unitOfWork.PurchaseOrderRepository
				.GetPurchaseOrdersBySupplierAndStatusAsync(supplier.SupplierId);

			if (!purchaseOrders.Any())
				return ApiResponse<List<PendingPurchaseOrderResponseDto>>.Success(null,200,"No pending purchase orders found for processing.");

			var pendingOrders = purchaseOrders.Select(po => new PendingPurchaseOrderResponseDto
			{
				PurchaseOrderId = po.PurchaseOrderId,
				Status = po.PurchaseOrderStatus.ToString()=="Sent"? "Pending (Not processed)": "partially received to Inventory"
			}).ToList();

			return ApiResponse<List<PendingPurchaseOrderResponseDto>>.Success(pendingOrders, 200 );
		}

	}
}