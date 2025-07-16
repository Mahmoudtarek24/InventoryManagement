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
		private readonly IUserContextService userContextService ;	
		public CategoryServices(IUnitOfWork unitOfWork,IUriService uriService,IUserContextService userContextService)
		{
			this.unitOfWork = unitOfWork ;		
			this.uriService = uriService ;
			this.userContextService = userContextService ;	
		}
		
		public async Task<ApiResponse<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto)
		{
			bool nameExists = await unitOfWork.CategoryRepository.IsCategoryNameUniqueAsync(dto.Name);

			if (nameExists)
				return ApiResponse<CategoryResponseDto>.ValidationError($"Category with name '{dto.Name}' already exists.");

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
			if (category is null)
				return ApiResponse<CategoryResponseDto>.Failuer(404,$"Category with Id '{dto.CategoryId}' Not Found ");

			bool nameExists = await unitOfWork.CategoryRepository.IsCategoryNameUniqueAsync(dto.Name);
			if (nameExists)
				return ApiResponse<CategoryResponseDto>.ValidationError($"Category with name '{dto.Name}' already exists.");

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
			if (category is null)
				throw new Exception();

			var response = category.ToResponseDto();
			return ApiResponse<CategoryResponseDto>.Success(response, 200);
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> SoftDeleteCategoryAsync(int id)
		{
			var category = await unitOfWork.CategoryRepository.GetByIdAsync(id);
			if (category is null)
				return ApiResponse<ConfirmationResponseDto>.Failuer(404, $"Category with Id '{id}' Not Found ");

			var hasProducts = await unitOfWork.ProductRepository.HasProductsForCategoryAsync(id);
			if (hasProducts)
				return ApiResponse<ConfirmationResponseDto>
					          .Failuer(401, $"Cannot delete category with ID {id} because it has related products.");
			
			await unitOfWork.BeginTransactionAsync();
			category.LastUpdateOn = DateTime.Now;
			category.IsDeleted = !category.IsDeleted;
			await unitOfWork.CommitTransaction();

			var action = category.IsDeleted ? "soft-deleted" : "restored";
			var status = category.IsDeleted ? ConfirmationStatus.SoftDeleted : ConfirmationStatus.Restored;

			ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			{
				Message = $"Category with ID {category.CategoryId} was {action} successfully.",
				status = status
			};

			return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 200);
		}

		public async Task<PagedResponse<List<CategoryResponseDto>>> GetCategoriesWithPaginationAsync(PaginationQueryParameters paginationQuery)
		{
			var parameter = new BaseFilter()
			{
				PageNumber = paginationQuery.PageNumber,
				PageSize = paginationQuery.PageSize,
			};

			var (categories, totalCount) =await unitOfWork.CategoryRepository.GetCategorysWithFiltersAsync(parameter);

			var categoryDtos = categories.Select(c => c.ToResponseDto()).ToList();
			var pagedResult = PagedResponse<List<CategoryResponseDto>>.SimpleResponse(categoryDtos, parameter.PageNumber, parameter.PageSize, totalCount);
			return pagedResult.AddPagingInfo(totalCount, uriService, userContextService.Route);
		}
		  
		public async Task<ApiResponse<List<CategoryResponseDto>>> GetAllCategoryAsync()
		{
			var categories = await unitOfWork.CategoryRepository.GetAllActiveCategoryAsync();
			
			if (categories is null || !categories.Any())
				return ApiResponse<List<CategoryResponseDto>>.Success(null, 200, "No categories found");

			var categoryDtos = categories.Select(c => c.ToResponseDto()).ToList();
			return ApiResponse<List<CategoryResponseDto>>.Success(categoryDtos, 200);
		}
	}
}
