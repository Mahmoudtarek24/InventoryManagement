using Application.DTO_s;
using Application.DTO_s.WarehouseDto_s;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.WarehouseResponse;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface IWarehouseService
	{
		Task<ApiResponse<ConfirmationResponseDto>> CreateWarehouseAsync(CreateWarehouseDto dto);
		Task<ApiResponse<WarehouseResponseDto>> GetWarehouseByIdAsync(int id);
		Task<PagedResponse<List<WarehouseResponseDto>>> GetWarehousesAsync(PaginationQueryParameters query);
		Task SendProductsToWarehouse(PurchaseOrder purchaseOrder);
	}
}
