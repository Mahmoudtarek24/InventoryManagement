using Application.Constants.Enum;
using Application.DTO_s;
using Application.DTO_s.PurchaseOrder;
using Application.Interfaces;
using Application.Mappings;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.PurchaseOrder;
using Application.ResponseDTO_s.SupplierResponse;
using Domain.Entity;
using Domain.Enum;
using Domain.Interface;
using Domain.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
	public class PurchaseOrderService : IPurchaseOrderService
	{
		Dictionary<string, List<PurchaseOrderStatus>> ValidOrderStatus
					  = new Dictionary<string, List<PurchaseOrderStatus>>()
					  {
						  {"Create",new List<PurchaseOrderStatus>{ PurchaseOrderStatus.Draft ,PurchaseOrderStatus.Sent} },
						  {"Update",new List<PurchaseOrderStatus>{ PurchaseOrderStatus.Draft ,PurchaseOrderStatus.Sent,
						  PurchaseOrderStatus.Cancelled} },
					  };

		private readonly IUnitOfWork unitOfWork;
		private readonly IUserContextService userContextService;
		private readonly IUriService uriService;
		private readonly RoleBasedPurchaseOrderMapper roleBasedPurchaseOrderMapper;
		public PurchaseOrderService(IUnitOfWork unitOfWork, IUserContextService userContextService 
			                        , RoleBasedPurchaseOrderMapper roleBasedPurchaseOrderMapper,IUriService uriService)
		{
			this.unitOfWork = unitOfWork;
			this.userContextService = userContextService;
			this.roleBasedPurchaseOrderMapper = roleBasedPurchaseOrderMapper;
			this.uriService = uriService;
		}
		public async Task<ApiResponse<ConfirmationResponseDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto)
		{
			List<string> warnings = new List<string>();

			var key = "Create";
			if (ValidOrderStatus[key].Contains(dto.purchaseOrderStatus))
				throw new Exception();
			//throw new BadRequestException($"Invalid purchase order status '{dto.purchaseOrderStatus}' for operation '{context}'.");

			bool supplierExists = await unitOfWork.SupplierRepository.IsVerifiedAndActiveSupplierAsync(dto.SupplierId);
			if (!supplierExists)
				throw new Exception();
			//  throw new NotFoundException($"Category with ID '{dto.CategoryId}' not found.");

			var supplierProductIds = await unitOfWork.ProductRepository.GetProductsBySupplierAsync(dto.SupplierId);
			var supplierProductSet = new HashSet<int>(supplierProductIds);

			var validOrderItems = dto.purchaseOrderItemDtos
						.Where(item => supplierProductSet.Contains(item.ProductId) && item.OrderQuantity > 0).ToList();

			var invalidProductIds = dto.purchaseOrderItemDtos.Select(p => p.ProductId)
							   .Where(id => !supplierProductSet.Contains(id)).ToList();

			if (invalidProductIds.Any())
				warnings.Add($"Products with IDs [{string.Join(", ", invalidProductIds)}] are not valid for the selected supplier.");

			var purchaseOrder = new PurchaseOrder
			{
				ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
				CreateOn = DateTime.Now,
				IsDeleted = false,
				SupplierId = dto.SupplierId,
				PurchaseOrderStatus = dto.purchaseOrderStatus,
				WarehouseId=dto.WarehouseId,	
			};

			var productPrices = await unitOfWork.ProductRepository
								 .GetProductPricesAsync(validOrderItems.Select(e => e.ProductId).Distinct().ToList());
			foreach (var item in validOrderItems)
			{
				purchaseOrder.OrderItems.Add(new PurchaseOrderItem
				{
					OrderQuantity = item.OrderQuantity,
					ProductId = item.ProductId,
					UnitPrice = productPrices[item.ProductId]
				});
			}
			purchaseOrder.TotalCost = purchaseOrder.OrderItems.Sum(e => e.UnitPrice * e.OrderQuantity);

			await unitOfWork.BeginTransactionAsync();
			await unitOfWork.PurchaseOrderRepository.AddAsync(purchaseOrder);
			await unitOfWork.CommitTransaction();

			ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			{
				Message = $"Purchase order for supplier #{dto.SupplierId} was successfully created with {purchaseOrder.OrderItems.Count} items. Expected delivery: {dto.ExpectedDeliveryDate:yyyy-MM-dd}.",
				status = ConfirmationStatus.Created
			};
			return ApiResponse<ConfirmationResponseDto>.SuccessWithWarnings(responseDto, 200, warnings);
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> UpdateDraftPurchaseOrderAsync(UpdatePurchaseOrderDto dto)
		{
			var PurchaseOrder = await unitOfWork.PurchaseOrderRepository.GetPurchaseOrderWithItemsAsync(dto.PurchaseOrderId);
			if (PurchaseOrder is null || !PurchaseOrder.OrderItems.Any())
				throw new Exception();


			if (!ValidOrderStatus["Update"].Contains(dto.purchaseOrderStatus.Value))
			{
				throw new Exception();
			}

			if (dto.purchaseOrderStatus == PurchaseOrderStatus.Cancelled)
			{
				var responseDto = new ConfirmationResponseDto()
				{
					Message = "cancelled",
					status = ConfirmationStatus.Cancelled
				};
				return ApiResponse<ConfirmationResponseDto>.Success(responseDto,200);
			}

			var oldDict = PurchaseOrder.OrderItems.OrderBy(e=>e.ProductId)
				                             .ToDictionary(e => e.ProductId, e => e.OrderQuantity);

			var newDict = dto.purchaseOrderItemDtos.OrderBy(e=>e.ProductId)
				                           .ToDictionary(e => e.ProductId, e => e.OrderQuantity);

			//bool areEqual= oldDict.Count() == newDict.Count()&&oldDict.SequenceEqual(newDict);
			//if (areEqual)
			//{
				//var productPrices = await unitOfWork.ProductRepository
				//		 .GetProductPricesAsync(dto.purchaseOrderItemDtos.Select(e => e.ProductId).Distinct().ToList());
				//foreach (var item in PurchaseOrder.OrderItems)
				//{
				//	item.UnitPrice= productPrices[item.ProductId];	
				//}
				////////we dint get any changed on order items
				//PurchaseOrder.PurchaseOrderStatus = dto.purchaseOrderStatus?? PurchaseOrderStatus.Draft;
				//PurchaseOrder.ExpectedDeliveryDate=dto.ExpectedDeliveryDate;
				//PurchaseOrder.LastUpdateOn=DateTime.Now;
				//PurchaseOrder.TotalCost = PurchaseOrder.OrderItems.Sum(e => e.UnitPrice);
			//}
			return default;
		}

		public async Task<ApiResponse<PurchaseOrderDetailsResponseDto>> GetPurchaseorderByIdAsync(int purchaseId)
		{
			var purchaseOrder = await unitOfWork.PurchaseOrderRepository.GetPurchaseOrderWithItemsAndSupplierAsync(purchaseId);
			if (purchaseOrder == null || purchaseOrder.IsDeleted)
				throw new Exception("Purchase order not found.");

			if (userContextService.IsSupplier)
			{
				var supplier = await unitOfWork.SupplierRepository.GetSupplierByUserIdAsync(userContextService.userId);
				if (supplier == null)
					throw new Exception("Supplier not found for current user.");

				// Check if the purchase order belongs to this supplier
				if (purchaseOrder.SupplierId != supplier.SupplierId)
					throw new Exception("Access denied. You can only view your own purchase orders.");

				// Check if supplier can view this purchase order based on status
				var allowedStatuses = new[] {
						PurchaseOrderStatus.Sent,
						PurchaseOrderStatus.PartiallyReceived,
						PurchaseOrderStatus.Received };
			
				if (!allowedStatuses.Contains(purchaseOrder.PurchaseOrderStatus))
					throw new Exception("Access denied. You can only view purchase orders that have been sent to you.");
			}

			var response = roleBasedPurchaseOrderMapper.MapToPurchaseOrderDetailsDtoAsync(purchaseOrder);
			return ApiResponse<PurchaseOrderDetailsResponseDto>.Success(response, 200);
		}

		public async Task<PagedResponse<List<PurchaseOrderListItemResponseDto>>> GetPurchaseOrdersWithPaginationAsync
			                                       (PurchaseOrderQueryParameter queryParam, string route)
		{
			var filter = new PurchaseOrderFilter
			{
				PageNumber = queryParam.PageNumber,
				PageSize = queryParam.PageSize,
				searchTearm = queryParam.searchTearm,
				SortAscending = queryParam.SortAscending,
				SortBy = queryParam.SortOptions.ToString(),
				Status = queryParam.Status 
			};
			var (totalCount, purchaseOrders) = await unitOfWork.PurchaseOrderRepository.GetPurchaseOrdersWithFiltersAsync(filter);

			//queryParam.TotalCount = totalCount;

			if (totalCount == 0)
			{
				return PagedResponse<List<PurchaseOrderListItemResponseDto>>.SimpleResponse(
					new List<PurchaseOrderListItemResponseDto>(),
					queryParam.PageNumber,
					queryParam.PageSize,
					0,
					"No purchase orders found matching the specified criteria.");
			}

			var purchaseOrderDtos = purchaseOrders.Select(e=>e.ToResponseDto()).ToList();

			var pagedResponse = PagedResponse<List<PurchaseOrderListItemResponseDto>>.SimpleResponse(purchaseOrderDtos,
				queryParam.PageNumber,
				queryParam.PageSize,
				totalCount);

			return pagedResponse.AddPagingInfo(totalCount, uriService,route);
		}

		public async Task<List<PurchaseOrderBySupplierResponseDto>> GetOrdersBySupplierAsync(int supplierId)
		{
			var purchaseOrders = await unitOfWork.PurchaseOrderRepository.GetPurchaseOrdersBySupplierAsync(supplierId);

			var purchaseOrderDtos = purchaseOrders.Select(po => new PurchaseOrderBySupplierResponseDto
			{
				PurchaseOrderId = po.PurchaseOrderId,
				ExpectedDeliveryDate = po.ExpectedDeliveryDate,
				TotalCost = po.TotalCost,
				PurchaseOrderStatus = po.PurchaseOrderStatus,
				NumberOfItems = po.OrderItems?.Count ?? 0
			}).ToList();

			return purchaseOrderDtos;
		}
	}
}
