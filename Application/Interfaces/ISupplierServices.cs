using Application.DTO_s.SupplierDto_s;
using Application.ResponseDTO_s.SupplierResponse;
using Application.ResponseDTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface ISupplierServices
	{
		Task<ApiResponse<ConfirmationResponseDto>> CreateSupplierAsync(CreateSupplierDto dto);
		Task<ApiResponse<SupplierResponseDto>> GetSupplierByIdAsync(int id, string[] Roles, ProductPaginationForSupplierQuery queryParamter);
		Task<PagedResponse<List<SupplierListRespondDto>> GetAllSuppliersAsync(string Roles, SupplierQueryParameters qP);
		//Task<ApiResponse<CategoryResponseDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
		//Task<ApiResponse<ConfirmationResponseDto>> SoftDeleteCategoryAsync(int id);
	}
}
