using Domain.Entity;
using Domain.Parameters;
using Domain.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
	public interface IInventoryRepository :IGenaricRepository<Inventory>
	{
		Task<Inventory?> GetInventoryByProductAndWarehouseAsync(int productId, int warehouseId);
		Task<(List<InventoryInfo>, int)> GetInventoryByWarehouseWithFiltersAsync(int warehouseId, BaseFilter filter);
		Task<List<InventoryInfo>> GetInventoryByProductAsync(int productId);
		Task<List<InventoryInfo>> GetLowStockItemsAsync(int threshold);
		Task<List<Inventory>> GetInventorysByProductsAndWarehousesAsync(List<int> productId, List<int> warehouseId);
	}
}
