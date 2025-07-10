using Application.Constants.Enum;
using Application.DTO_s;
using Application.DTO_s.ProductDto_s;
using Application.Exceptions;
using Application.Interfaces;
using Application.Mappings;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.CategoryResponse;
using Application.ResponseDTO_s.ProductResponse;
using Application.ResponseDTO_s.PurchaseOrder;
using Domain.Entity;
using Domain.Interface;
using Domain.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services
{
	public class ProductServices : IProductServices
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IUriService uriService;
		private readonly IUserContextService userContextService;
		private readonly RoleBasedSupplierMapper mapper;
		public ProductServices(IUnitOfWork unitOfWork, IUriService uriService, IUserContextService userContextService
			                  ,RoleBasedSupplierMapper mapper)
		{
			this.unitOfWork = unitOfWork;
			this.uriService = uriService;
			this.userContextService = userContextService;
			this.mapper = mapper;
		}

		public async Task<ApiResponse<ProductResponseDto>> CreateProductAsync(CreateProductDto dto)
		{
			bool categoryExists = await unitOfWork.CategoryRepository.IsValidCategoryIdAsync(dto.CategoryId);
			if (!categoryExists)
				throw new Exception();
			//  throw new NotFoundException($"Category with ID '{dto.CategoryId}' not found.");

			bool nameExists = await unitOfWork.ProductRepository.IsDuplicateProductNameInCategoryAsync(dto.Name, dto.CategoryId);
			if (nameExists)
				throw new Exception();
			//throw new ConflictException($"Product with name '{dto.Name}' already exists.");

			var supplier= await unitOfWork.SupplierRepository.GetSupplierByUserIdAsync(userContextService.userId);
			var product = new Product
			{
				Name = dto.Name,
				Price = dto.Price,
				CategoryId = dto.CategoryId,
				CreateOn = DateTime.Now,
				IsAvailable = true,
				SupplierId=supplier.SupplierId
			};
			product.Barcode = await GenerateBarcode(product);

			await unitOfWork.BeginTransactionAsync();
			await unitOfWork.ProductRepository.AddAsync(product);
			await unitOfWork.CommitTransaction();

			var response = product.ToResponseDto();
			return ApiResponse<ProductResponseDto>.Success(response, 201, $"Product with id: {product.ProductId} created successfully");
		}
		public async Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(int id)
		{
			var product = await unitOfWork.ProductRepository.GetByIdAsync(id);
			if (product == null)
				throw new Exception();
			//throw new NotFoundException($"Product with ID '{id}' not found.");

			var response = product.ToResponseDto();
			return ApiResponse<ProductResponseDto>.Success(response, 200, "Product retrieved successfully");
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> SoftDeleteProductAsync(int id)
		{
			var product = await unitOfWork.ProductRepository.GetIfExistsAndNotDeletedAsync(id);
			if (product == null)
				throw new Exception();
			//throw new ConflictException($"Category with name '{dto.Name}' already exists.");

			/////////////////////////////////////////////
			///TODO check if this product have quentity on stock then when i make soft delete to it 
			///do something like alert to admin know that that product have soft deleted but still have quentity on stock  
			/////////////////////////////////////////

			//////////////////////////////////////////////
			///TODO make hard delete but only product dint have any operation (like required for supplier , saild before)
			//////////////////////////////////////////////

			await unitOfWork.BeginTransactionAsync();
			product.LastUpdateOn = DateTime.Now;
			product.IsDeleted = !product.IsDeleted;
			product.IsAvailable = !product.IsAvailable;
			await unitOfWork.CommitTransaction();

			ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			{
				Message = $"Product with ID {product.ProductId} was marked as deleted.",
				status = ConfirmationStatus.SoftDeleted
			};

			return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 200);
		}
		public async Task<PagedResponse<List<ProductResponseDto>>> GetProductsWithPaginationAsync(BaseQueryParameters productQuery)
		{
			var parameter = new BaseFilter()
			{
				PageNumber = productQuery.PageNumber,
				PageSize = productQuery.PageSize,
				searchTearm = productQuery.searchTearm,
				SortAscending = productQuery.SortAscending,
				SortBy = productQuery.SortBy,
			};

			var (products, totalCount) = await unitOfWork.ProductRepository.GetProductsWithFiltersAsync(parameter);
			var productDtos = products.Select(c => c.ToResponseDto()).ToList();
			var message = !productDtos.Any() ? "No products found matching your criteria." : null;

			var pagedResult = PagedResponse<List<ProductResponseDto>>.SimpleResponse(productDtos, parameter.PageNumber, parameter.PageSize, totalCount, message);
			return pagedResult.AddPagingInfo(totalCount, uriService, userContextService.Route);
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> ChangeAvailabilityAsync(int productId, bool status)
		{
			var product = await unitOfWork.ProductRepository.GetByIdAsync(productId);
			if (product == null)
				throw new Exception();
			//throw new NotFoundException($"Product with ID '{productId}' not found.");

			// Check if the product is already in the requested status
			if (product.IsAvailable == status)
			{
				var currentStatusMessage = status ? "available" : "unavailable";
				//throw new ConflictException($"Product with ID {productId} is already {currentStatusMessage}.");
			}

			await unitOfWork.BeginTransactionAsync();
			product.LastUpdateOn = DateTime.UtcNow;
			product.IsAvailable = status;
			await unitOfWork.CommitTransaction();

			var statusText = status ? "available" : "unavailable";
			var confirmationStatus = status ? ConfirmationStatus.Activated : ConfirmationStatus.Deactivated;
			ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			{
				Message = $"Product '{product.Name}' with ID {product.ProductId} was marked as {statusText}.",
				status = confirmationStatus
			};
			return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 200);
		}
		public async Task<ApiResponse<List<ProductWithCategoryRespondDto>>> GetProductsByCategoryAsync(int categoryId)
		{
			var category = await unitOfWork.CategoryRepository.GetIfExistsAndNotDeletedAsync(categoryId);
			if (category is null)
				throw new Exception();
			//throw new NotFoundException($"Category with ID '{categoryId}' not found.");

			// Get products by category
			var products = await unitOfWork.ProductRepository.GetProductsByCategoryAsync(categoryId);

			var response = products.Select(p => p.ToResponseDtoWithCategory()).ToList();

			var message = response.Any()
				? $"Found {response.Count} product(s) for category ID {categoryId}."
				: $"No products found for category ID {categoryId}.";

			return ApiResponse<List<ProductWithCategoryRespondDto>>.Success(response, 200, message);
		}


		public async Task<ApiResponse<List<ProductResponseDto>>> BulkCreateProductsAsync(List<CreateProductDto> dtos)
		{
			var Warrning = new List<string>();
			var validDtos = new List<CreateProductDto>();

			var categoryIds = dtos.Select(dto => dto.CategoryId).Distinct().ToList();
			var validCategoryIds = await unitOfWork.CategoryRepository.GetValidCategoryIdsAsync(categoryIds);

			if (!validCategoryIds.Any())
				throw new Exception();
				///throw new NotFoundException("No valid categories found. All provided CategoryIds are invalid.");

			foreach (var dto in dtos)
			{
				if (!validCategoryIds.Contains(dto.CategoryId))
					Warrning.Add($"Category with ID '{dto.CategoryId}' not found.");
				else
					validDtos.Add(dto); 
			}

			var productNamesByCategory = validDtos.GroupBy(e => e.CategoryId)
		                           .ToDictionary(e=> e.Key, e => e.Select(e => e.Name).ToList());

			var existingProducts = await unitOfWork.ProductRepository
                                    		.GetExistingProductNamesInCategoriesAsync(productNamesByCategory);
			
			var supplier = await unitOfWork.SupplierRepository.GetSupplierByUserIdAsync(userContextService.userId);

			var finalValidDtos = new List<CreateProductDto>();
			foreach (var dto in dtos)
			{
				if (existingProducts.ContainsKey(dto.CategoryId) &&
						   existingProducts[dto.CategoryId].Contains(dto.Name))
					Warrning.Add($"Product with name '{dto.Name}' already exists in category '{dto.CategoryId}'.");
				else
					finalValidDtos.Add(dto);
			}

			if (!Warrning.Any())
				throw new Exception();
			//return ApiResponse<List<ProductResponseDto>>.Error(Warrning);.//////////////////////////////

			var products = finalValidDtos.Select(e=> new Product
			{
				Name = e.Name,
				Price = e.Price,
				CategoryId = e.CategoryId,
				CreateOn = DateTime.Now,
				IsAvailable = true,
				SupplierId = supplier.SupplierId
			}).ToList();

			await unitOfWork.BeginTransactionAsync();
			await unitOfWork.ProductRepository.AddRangeAsync(products);
			await unitOfWork.CommitTransaction();

		
			var responseDtos = products.Select(e=>e.ToResponseDto()).ToList();	
			return ApiResponse<List<ProductResponseDto>>.Success(responseDtos,201);
		}
		private async Task<string> GenerateBarcode(Product product) ///// convert it to back ground job
		{
			string barcode;
			do
			{
				var Number = new Random().Next(0001, 9999);
				barcode = $"CT{product.CategoryId:D2}-Sup{product.SupplierId:D2}-{product.CreateOn:yyMM}-{Number}";

			} while (!await unitOfWork.ProductRepository.IsBarcodeUniqueAsync(barcode));
			return barcode;
		}
		public async Task<ApiResponse<List<PurchaseHistoryProductResponseDto>>> GetProductPurchaseHistoryAsync(int productId)
		{
			var purchaseHistoryItems = await unitOfWork.PurchaseOrderItemRepository.GetPurchaseHistoryByProductIdAsync(productId);

			if (!purchaseHistoryItems.Any())
				return ApiResponse<List<PurchaseHistoryProductResponseDto>>.Success(
					new List<PurchaseHistoryProductResponseDto>(),
					200,
					"No purchase history found for this product.");

			var historyDtos = purchaseHistoryItems.Select(e=>e.ToResponseDto()).ToList();	

			return ApiResponse<List<PurchaseHistoryProductResponseDto>>.Success(historyDtos,200,
				                      $"Found {historyDtos.Count} purchase history records for the product.");
		}

		public async Task<PagedResponse<List<ProductsBySupplierResponseDto>>> GetProductsBySupplierAsync(string supplierId, bool IsSupplier, SupplierProductsQueryParameters qP)
		{
			string actualSupplierId = supplierId;
			if (IsSupplier)
			{
				var supplier = await unitOfWork.SupplierRepository.GetSupplierByUserIdAsync(supplierId);
				if (supplier == null)
				{
					throw new UnauthorizedAccessException("Supplier not found");
				}
				actualSupplierId = supplier.SupplierId.ToString();
			}
			var parameter = new BaseFilter()
			{
				PageNumber = qP.PageNumber,
				PageSize = qP.PageSize,
				searchTearm = qP.searchTearm,
				SortAscending = qP.SortAscending,
				SortBy = qP.SortOption.ToString(),
			};
			var (totalCount, products) = await unitOfWork.ProductRepository
				              .GetProductsBySupplierAsync(int.Parse(actualSupplierId), parameter);
			var productDtos = products.Select(product => mapper.MapProductToSupplierResponseDto(product)).ToList();

			return PagedResponse<List<ProductsBySupplierResponseDto>>
				           .SimpleResponse(productDtos, qP.PageNumber, qP.PageSize, totalCount);
		}
	}
}