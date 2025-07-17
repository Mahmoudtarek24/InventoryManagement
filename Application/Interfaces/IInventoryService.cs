using Application.DTO_s;
using Application.DTO_s.InventoryDto_s;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.InventoryResponse;
using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface IInventoryService 
	{
		Task<ApiResponse<InventoryResponseDto>> GetInventoryByProductAndWarehouseAsync(int productId, int warehouseId);
		Task<PagedResponse<List<InventoryResponseDto>>> GetInventoryByWarehouseAsync(InventorQueryParameter query, int warehouseId);
		Task<ApiResponse<ProductInventoryResponseDto>> GetInventoryByProductAsync(int productId);
		Task<ApiResponse<List<LowStockAlertDto>>> GetLowStockAlertsAsync(int threshold);
	}
}
