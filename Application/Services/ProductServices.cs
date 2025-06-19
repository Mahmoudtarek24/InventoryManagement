using Application.DTO_s.ProductDto_s;
using Application.Interfaces;
using Application.Mappings;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.CategoryResponse;
using Application.ResponseDTO_s.ProductResponse;
using Domain.Entity;
using Domain.Interface;
using Domain.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
	public class ProductServices : IProductServices
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IUriService uriService;
		public ProductServices(IUnitOfWork unitOfWork, IUriService uriService)
		{
			this.unitOfWork = unitOfWork;
			this.uriService = uriService;
		}
		public async Task<ApiResponse<ProductResponseDto>> CreateProductAsync(CreateProductDto dto)
		{
			bool categoryExists = await unitOfWork.CategoryRepository.IsValidCategoryIdAsync(dto.CategoryId);
			if (!categoryExists)
				throw new Exception();
			//  throw new NotFoundException($"Category with ID '{dto.CategoryId}' not found.");

			bool nameExists = await unitOfWork.ProductRepository.IsDuplicateProductNameInCategoryAsync(dto.Name, dto.CategoryId);
			if (!nameExists)
				throw new Exception();
			//throw new ConflictException($"Product with name '{dto.Name}' already exists.");

			var product = new Product
			{
				Name = dto.Name,
				Price = dto.Price,
				CategoryId = dto.CategoryId,
				CreateOn = DateTime.Now,
				IsAvailable = true,
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
		public async Task<PagedResponse<List<ProductResponseDto>>> GetProductsWithPaginationAsync(ProductQueryParameters productQuery, string route)
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
			var message = !productDtos.Any() ? "No products found matching your criteria." : null ;

			var pagedResult = PagedResponse<List<ProductResponseDto>>.SimpleResponse(productDtos, parameter.PageNumber, parameter.PageSize, totalCount,message);
			return pagedResult.AddPagingInfo(totalCount, uriService, route);
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
			List<string> Errors = new List<string>();

			foreach (var dto in dtos)
			{
				bool categoryExists = await unitOfWork.CategoryRepository.IsValidCategoryIdAsync(dto.CategoryId);
				if (!categoryExists)
					Errors.Add($"Category with ID '{dto.CategoryId}' not found.");
			}
			foreach (var dto in dtos)
			{
				bool nameExists = await unitOfWork.ProductRepository.IsDuplicateProductNameInCategoryAsync(dto.Name, dto.CategoryId);
				if (!nameExists)
					Errors.Add($"Product with name '{dto.Name}' already exists.");
			}

			return default;
		}
		private async Task<string> GenerateBarcode(Product product)
		{
			string barcode;
			do
			{
				var Number = new Random().Next(0001, 9999);
				barcode = $"CT{product.CategoryId:D2}-Sup11-{product.CreateOn.ToString("yyMM")}-{Number}";

			} while (!await unitOfWork.ProductRepository.IsBarcodeUniqueAsync(barcode));
			return barcode;
		}
	}
}