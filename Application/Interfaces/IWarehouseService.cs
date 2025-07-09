using Application.DTO_s;
using Application.DTO_s.WarehouseDto_s;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.WarehouseResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface IWarehouseService
	{
		Task<ApiResponse<ConfirmationResponseDto>> CreateCategoryAsync(CreateWarehouseDto dto);
		Task<ApiResponse<WarehouseResponseDto>> GetWarehouseByIdAsync(int id);
		Task<PagedResponse<List<WarehouseResponseDto>>> GetWarehousesAsync(BaseQueryParameters query, string route);
	}
}
