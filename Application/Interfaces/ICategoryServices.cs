using Application.DTO_s;
using Application.DTO_s.CategoryDto_s;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.CategoryResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface ICategoryServices
	{
		Task<ApiResponse<CategoryResponseDto>> CreateCategoryAsync(CreateCategoryDto dto);
		Task<ApiResponse<CategoryResponseDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
		//Task<ApiResponse<CategoryResponseDto>> GetCategoryByIdAsync(int id);
		Task<ApiResponse<ConfirmationResponseDto>> SoftDeleteCategoryAsync(int id);
		Task<PagedResponse<List<CategoryResponseDto>>> GetCategoriesWithPaginationAsync(PaginationQueryParameters paginationQuery);
		Task<ApiResponse<List<CategoryResponseDto>>> GetAllCategoryAsync();
	}
}
