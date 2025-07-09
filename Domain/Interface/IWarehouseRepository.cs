using Domain.Entity;
using Domain.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
	public interface IWarehouseRepository :IGenaricRepository<Warehouse>
	{
		Task<bool> ExistsAsync(int warehouseId);
		Task<string?> GetLastSerialNumberByGovernorateAsync(string governorate);
		Task<(List<Warehouse>, int)> GetWarehousesWithFiltersAsync(BaseFilter filter);
	}
}
