using Application.Constants.Enum;
using Application.DTO_s;
using Application.DTO_s.CategoryDto_s;
using Application.Interfaces;
using Application.Mappings;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.CategoryResponse;
using Domain.Entity;
using Domain.Interface;
using Domain.Parameters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Services
{
	public class CategoryServices :ICategoryServices
	{
		private readonly IUnitOfWork unitOfWork ;
		private readonly IUriService uriService ;	
		public CategoryServices(IUnitOfWork unitOfWork,IUriService uriService)
		{
			this.unitOfWork = unitOfWork ;		
			this.uriService = uriService ;
		}
		
		public async Task<ApiResponse<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto)
		{
			var validationErrors = new Dictionary<string, string[]>();
			bool nameExists = await unitOfWork.CategoryRepository.IsCategoryNameUniqueAsync(dto.Name);
			if (nameExists)
			{
				validationErrors.Add("Name",new[] { "Category with name '{dto.Name}' already exists." });
				return ApiResponse<CategoryResponseDto>.ValidationError(validationErrors);	
			}

			var category = new Category
			{
				Description = dto.Description,
				CreateOn = DateTime.Now,
				DisplayOrder = dto.DisplayOrder,
				Name = dto.Name,
			};
			await unitOfWork.BeginTransactionAsync();
			await unitOfWork.CategoryRepository.AddAsync(category);
			await unitOfWork.CommitTransaction();

			var response = category.ToResponseDto();
			return ApiResponse<CategoryResponseDto>.Success(response, 200, $"category with id : {category.CategoryId} Created Successfully");
		}

		public async Task<ApiResponse<CategoryResponseDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto) //work with validation and concat upade/create CategoryInputDto
		{
			var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
			if (category == null)
				throw new Exception();
			//throw new ConflictException($"Category with name '{dto.Name}' already exists.");

			bool nameExists = await unitOfWork.CategoryRepository.IsCategoryNameUniqueAsync(dto.Name);
			if (nameExists)
				throw new Exception();
			//throw new ConflictException($"Category with name '{dto.Name}' already exists.");

			
			await unitOfWork.BeginTransactionAsync();
			category.Name = dto.Name;
			category.Description = dto.Description;
			category.DisplayOrder = dto.DisplayOrder;
			category.LastUpdateOn = DateTime.Now;	
			await unitOfWork.CommitTransaction();

			var response = category.ToResponseDto();
			return ApiResponse<CategoryResponseDto>.Success(response, 200, $"category with id : {category.CategoryId} Update Successfully");
		}

		public async Task<ApiResponse<CategoryResponseDto>> GetCategoryByIdAsync(int id)
		{
			var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
			if (category == null)
				throw new Exception();
			//throw new ConflictException($"Category with name '{dto.Name}' already exists.");

			var response = category.ToResponseDto();
			return ApiResponse<CategoryResponseDto>.Success(response, 200);
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> SoftDeleteCategoryAsync(int id)
		{
			var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
			if (category is null)
				throw new Exception();
			//throw new ConflictException($"Category with name '{dto.Name}' already exists.");

			var hasProducts = await unitOfWork.ProductRepository.HasProductsForCategoryAsync(id);
			if (hasProducts)
				throw new Exception();
			//throw throw new ConflictException($"Cannot delete category with ID {id} because it has related products.");


			await unitOfWork.BeginTransactionAsync();
			category.LastUpdateOn = DateTime.Now;
			category.IsDeleted = !category.IsDeleted;
			await unitOfWork.CommitTransaction();

			var action = category.IsDeleted ? "soft-deleted" : "restored";
			ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			{
				Message = $"Category with ID {category.CategoryId} was {action} successfully.",
				status = ConfirmationStatus.SoftDeleted
			};

			return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 200);
		}

		public async Task<PagedResponse<List<CategoryResponseDto>>> GetCategoriesWithPaginationAsync(BaseQueryParameters categoryQuery,string route)
		{
			var parameter = new BaseFilter()
			{
				PageNumber = categoryQuery.PageNumber,
				PageSize = categoryQuery.PageSize,
				searchTearm = categoryQuery.searchTearm,
				SortAscending = categoryQuery.SortAscending,
				SortBy = categoryQuery.SortBy,
			};

			var (categories, totalCount) =await unitOfWork.CategoryRepository.GetCategorysWithFiltersAsync(parameter);

			if (categories == null || !categories.Any())
				throw new Exception();
			//throw new NotFoundException("No categories found.");

			var categoryDtos = categories.Select(c => c.ToResponseDto()).ToList();
			var pagedResult =  PagedResponse<List<CategoryResponseDto>>.SimpleResponse(categoryDtos, parameter.PageNumber, parameter.PageSize, totalCount);
			return pagedResult.AddPagingInfo(totalCount, uriService, route);
		}

		public async Task<ApiResponse<List<CategoryResponseDto>>> GetAllCategoryAsync()
		{
			var categories = await unitOfWork.CategoryRepository.GetAllActiveCategoryAsync();
			
			if (categories == null || !categories.Any())
				throw new Exception();
			//throw new NotFoundException("No categories found.");

			var categoryDtos = categories.Select(c => c.ToResponseDto()).ToList();
			return ApiResponse<List<CategoryResponseDto>>.Success(categoryDtos, 200);
		}
	}
}
