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
using System.Collections.Generic;

namespace Application.Services
{
	public class ProductServices : IProductServices
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IUriService uriService;
		private readonly IUserContextService userContextService;
		private readonly RoleBasedSupplierMapper mapper;
		public ProductServices(IUnitOfWork unitOfWork, IUriService uriService, IUserContextService userContextService
							  , RoleBasedSupplierMapper mapper)
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
				return ApiResponse<ProductResponseDto>.Failuer(404, $"Category with ID '{dto.CategoryId}' not found.");
			//  

			bool nameExists = await unitOfWork.ProductRepository.IsDuplicateProductNameInCategoryAsync(dto.Name, dto.CategoryId);
			if (nameExists)
				return ApiResponse<ProductResponseDto>.ValidationError($"Product with name '{dto.Name}' already exists.");

			var supplier = await unitOfWork.SupplierRepository.GetSupplierByUserIdAsync(userContextService.userId);
			var product = new Product
			{
				Name = dto.Name,
				Price = dto.Price,
				CategoryId = dto.CategoryId,
				CreateOn = DateTime.Now,
				IsAvailable = true,
				SupplierId = supplier.SupplierId
			};
			//product.Barcode = await GenerateBarcode(product);

			await unitOfWork.BeginTransactionAsync();
			await unitOfWork.ProductRepository.AddAsync(product);
			await unitOfWork.CommitTransaction();

			var response = product.ToResponseDto(userContextService);
			return ApiResponse<ProductResponseDto>.Success(response, 201, $"Product with id: {product.ProductId} created successfully");
		}

		public async Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(int id)
		{
			var product = await unitOfWork.ProductRepository.GetByIdAsync(id);
			if (product == null)
				return ApiResponse<ProductResponseDto>.Failuer(404, $"Category with ID '{id}' not found.");

			var response = product.ToResponseDto(userContextService);
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
			};

			var (products, totalCount) = await unitOfWork.ProductRepository.GetProductsWithFiltersAsync(parameter);
			var productDtos = products.Select(c => c.ToResponseDto(userContextService)).ToList();

			var message = !productDtos.Any() ? "No products found matching your criteria." : null;

			var pagedResult = PagedResponse<List<ProductResponseDto>>.SimpleResponse(productDtos, parameter.PageNumber, parameter.PageSize, totalCount, message);
			return pagedResult.AddPagingInfo(totalCount, uriService, userContextService.Route);
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> ChangeAvailabilityAsync(int productId, bool status)
		{
			var product = await unitOfWork.ProductRepository.GetByIdAsync(productId);
			if (product == null)
				return ApiResponse<ConfirmationResponseDto>.Failuer(404, $"Product with ID '{productId}' not found.");

			if (product.IsAvailable == status)
			{
				var currentStatusMessage = status ? "available" : "unavailable";
				return ApiResponse<ConfirmationResponseDto>.Success(null, 200, $"Product with ID {productId} is already {currentStatusMessage}.");
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
				return ApiResponse<List<ProductWithCategoryRespondDto>>.Failuer(404, $"Category with ID '{categoryId}' not found.");

			var products = await unitOfWork.ProductRepository.GetProductsByCategoryAsync(categoryId);

			var response = products.Select(p => p.ToResponseDtoWithCategory()).ToList();

			var message = response.Any()
				? $"Found {response.Count} product(s) for category ID {categoryId}."
				: $"No products found for category ID {categoryId}.";

			return ApiResponse<List<ProductWithCategoryRespondDto>>.Success(response, 200, message);
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

			var historyDtos = purchaseHistoryItems.Select(e => e.ToResponseDto()).ToList();

			return ApiResponse<List<PurchaseHistoryProductResponseDto>>.Success(historyDtos, 200,
									  $"Found {historyDtos.Count} purchase history records for the product.");
		}
		public async Task<ApiResponse<ProductsBySupplierResponseDto>> GetProductsBySupplierAsync(string supplierId, PaginationQueryParameters qP)
		{
			string actualSupplierId = supplierId;

			if (userContextService.IsSupplier)
			{
				var supplie = await unitOfWork.SupplierRepository.GetSupplierByUserIdAsync(supplierId);

				if (supplie is null)
					return ApiResponse<ProductsBySupplierResponseDto>.Failuer(404, "Supplier not found.");

				actualSupplierId = supplie.SupplierId.ToString();
			}

			var parameter = new BaseFilter()
			{
				PageNumber = qP.PageNumber,
				PageSize = qP.PageSize,
			};

			if (!int.TryParse(actualSupplierId, out int supplierIdInt))
				return ApiResponse<ProductsBySupplierResponseDto>.Failuer(400, "Invalid supplier ID format.");

			var (totalCount, products) = await unitOfWork.ProductRepository
												 .GetProductsBySupplierAsync(supplierIdInt, parameter);

			if (products == null || !products.Any())
				return ApiResponse<ProductsBySupplierResponseDto>.Success(null, 200, "No products found for this supplier.");

			var productDtos = products.Select(product => mapper.MapProductToResponseDto(product)).ToList();

			var supplier = products.FirstOrDefault(p => p.SupplierId == supplierIdInt)?.Supplier;

			var supplierDto = mapper.MapSupplierToResponseDto(supplier);

			var productPageResponse = PagedResponse<List<ProductBaseRespondDto>>
										.SimpleResponse(productDtos, qP.PageNumber, qP.PageSize, totalCount);

			supplierDto.SupplierProducts = productPageResponse;

			return ApiResponse<ProductsBySupplierResponseDto>.Success(supplierDto, 200);
		}

		public async Task<ApiResponse<List<ProductResponseDto>>> BulkCreateProductsAsync(List<CreateProductDto> dtos)
		{
			var warnings = new List<string>();
			var validDtos = new List<CreateProductDto>();

			var categoryIds = dtos.Select(dto => dto.CategoryId).Distinct().ToList();
			var validCategoryIds = await unitOfWork.CategoryRepository.GetValidCategoryIdsAsync(categoryIds);

			if (!validCategoryIds.Any())
				return ApiResponse<List<ProductResponseDto>>.ValidationError("No valid categories found. All provided CategoryIds are invalid.");

			foreach (var dto in dtos)
			{
				if (!validCategoryIds.Contains(dto.CategoryId))
					warnings.Add($"Category with ID '{dto.CategoryId}' not found.");
				else
					validDtos.Add(dto);
			}

			var productNamesByCategory = validDtos.GroupBy(e => e.CategoryId)
									   .ToDictionary(g => g.Key, g => g.Select(item => item.Name).ToList());

			var existingProducts = await unitOfWork.ProductRepository
										  .GetExistingProductNamesInCategoriesAsync(productNamesByCategory);

			var supplier = await unitOfWork.SupplierRepository.GetSupplierByUserIdAsync(userContextService.userId);
			if (supplier is null)
				return ApiResponse<List<ProductResponseDto>>.ValidationError("Supplier not found for current user.");

			var validatedItems = new List<CreateProductDto>();
			foreach (var dto in validDtos)
			{
				if (existingProducts.ContainsKey(dto.CategoryId) &&
					existingProducts[dto.CategoryId].Contains(dto.Name))
					warnings.Add($"Product with name '{dto.Name}' already exists in category '{dto.CategoryId}'.");
				else
					validatedItems.Add(dto);
			}

			if (!validatedItems.Any())
				return ApiResponse<List<ProductResponseDto>>.ValidationError(warnings);

			var products = validatedItems.Select(dto => new Product
			{
				Name = dto.Name,
				Price = dto.Price,
				CategoryId = dto.CategoryId,
				CreateOn = DateTime.UtcNow,
				IsAvailable = true,
				SupplierId = supplier.SupplierId
			}).ToList();

			await unitOfWork.BeginTransactionAsync();
			await unitOfWork.ProductRepository.AddRangeAsync(products);
			await unitOfWork.CommitTransaction();


			var responseDtos = products.Select(product => product.ToResponseDto(userContextService)).ToList();
			if (warnings.Any())
				return ApiResponse<List<ProductResponseDto>>.SuccessWithWarnings(responseDtos, 200, warnings);

			return ApiResponse<List<ProductResponseDto>>.Success(responseDtos, 200);
		}


		public async Task<ApiResponse<ConfirmationResponseDto>> BulkUpdateProductPricesAsync(List<UpdateProductPriceDto> dtos)
		{
			var warnings = new List<string>();
			var validatedItems = new List<(UpdateProductPriceDto dto, Product product)>();

			var productIds = dtos.Select(dto => dto.ProductId).Distinct().ToList();

			var supplier = await unitOfWork.SupplierRepository.GetSupplierByUserIdAsync(userContextService.userId);
			if (supplier == null)
				return ApiResponse<ConfirmationResponseDto>.Failuer(404, "Supplier not found for current user.");

			var existingProducts = await unitOfWork.ProductRepository
										  .GetProductsByIdsAndSupplierAsync(productIds, supplier.SupplierId);

			if (!existingProducts.Any())
				return ApiResponse<ConfirmationResponseDto>.Failuer(404, "No valid products found for the current supplier.");

			var productLookup = existingProducts.ToDictionary(p => p.ProductId, p => p);

			foreach (var dto in dtos)
			{
				if (!productLookup.TryGetValue(dto.ProductId, out var product))
				{
					warnings.Add($"Product with ID '{dto.ProductId}' not found or doesn't belong to current supplier.");
					continue;
				}

				if (dto.NewPrice <= 0)
				{
					warnings.Add($"Invalid price '{dto.NewPrice}' for Product ID '{dto.ProductId}'. Price must be greater than 0.");
					continue;
				}

				if (product.Price == dto.NewPrice)
				{
					warnings.Add($"Product ID '{dto.ProductId}' already has the same price '{dto.NewPrice}'.");
					continue;
				}

				validatedItems.Add((dto, product));
			}

			if (!validatedItems.Any())
				return ApiResponse<ConfirmationResponseDto>.ValidationError("No valid price updates could be processed.");

			await unitOfWork.BeginTransactionAsync();
			var totalUpdatedProducts = 0;
			var priceChangesDetails = new List<string>();

			foreach (var (updateDto, product) in validatedItems)
			{
				var oldPrice = product.Price;

				product.Price = updateDto.NewPrice;
				product.LastUpdateOn = DateTime.UtcNow;

				totalUpdatedProducts++;
				priceChangesDetails.Add($"Product ID {product.ProductId}: {oldPrice:C} → {updateDto.NewPrice:C}");
			}

			await unitOfWork.CommitTransaction();

			var successResponse = new ConfirmationResponseDto
			{
				Message = $"Price update completed successfully for {totalUpdatedProducts} products. {validatedItems.Count} out of {dtos.Count} products processed successfully.",
				status = ConfirmationStatus.register
			};

			if (warnings.Any())
				return ApiResponse<ConfirmationResponseDto>.SuccessWithWarnings(successResponse, 200, warnings);

			return ApiResponse<ConfirmationResponseDto>.Success(successResponse, 200);
		}
	}
}