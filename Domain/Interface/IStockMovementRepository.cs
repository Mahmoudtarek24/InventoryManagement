using Domain.Entity;
using Domain.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
	public interface IStockMovementRepository :IGenaricRepository<StockMovement>
	{
		Task<List<StockMovement>> GetStockMovementsByProductAsync(int productId, int pageNumber);
		Task<List<StockMovement>> GetStockMovementsByWarehouseAsync(int warehouseId, int pageNumber);
		Task<(int, List<StockMovement>)> GetStockMovementsWithFiltersAsync(StockMovementFilter query);

	}
}
