using Domain.Entity;
using Domain.Interface;
using Domain.Parameters;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
	public class WarehouseRepository : GenaricRepository<Warehouse>, IWarehouseRepository
	{
		public WarehouseRepository(InventoryManagementDbContext context) : base(context) { }

		public async Task<bool> ExistsAsync(int warehouseId) =>
				  await context.Warehouses.AnyAsync(w => w.WarehouseId == warehouseId);
		public async Task<string?> GetLastSerialNumberByGovernorateAsync(string governorate) =>
			    await context.Warehouses.Where(e => e.SerialNumber.StartsWith($"WH-{governorate}-"))
	            .OrderByDescending(e => e.WarehouseId).Select(e => e.SerialNumber).FirstOrDefaultAsync();

		public async Task<(List<Warehouse>, int)> GetWarehousesWithFiltersAsync(BaseFilter filter)
		{
			var query = context.Warehouses.AsQueryable();

			int totalCount = await query.CountAsync();
			query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

			var result = await query.ToListAsync();
			return (result, totalCount);
		}
		public override async Task<Warehouse?> GetByIdAsync(int id) =>
                await context.Warehouses.Include(e=>e.Inventories).SingleOrDefaultAsync(e=>e.WarehouseId == id);	
	}
}
