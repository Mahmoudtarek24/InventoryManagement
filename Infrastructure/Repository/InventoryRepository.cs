using Domain.Entity;
using Domain.Enum;
using Domain.Interface;
using Domain.Parameters;
using Domain.QueryParameters;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
	public class InventoryRepository :GenaricRepository<Inventory> , IInventoryRepository
	{
		public InventoryRepository(InventoryManagementDbContext context) : base(context) { }


		public async Task<Inventory?> GetInventoryByProductAndWarehouseAsync(int productId, int warehouseId) =>
		         await context.Inventories.Include(e=>e.Products).Include(e=>e.Warehouse)	
				 .Where(i => i.ProductId == productId && i.WarehouseId == warehouseId && !i.Products.IsDeleted)
				 .FirstOrDefaultAsync();

		public async Task<(List<InventoryInfo>, int)> GetInventoryByWarehouseWithFiltersAsync
																				   (int warehouseId, BaseFilter filter)
		{
			var query = context.Inventories.Include(e => e.Products)
				.Where(i => i.WarehouseId == warehouseId &&!i.Products.IsDeleted ).AsQueryable();

			if (!string.IsNullOrEmpty(filter.searchTearm))
				query = query.Where(i =>
					i.Products.Name.Contains(filter.searchTearm));

			int totalCount = await query.CountAsync();
	
			query = query.OrderBy(i => i.Products.Name);

			var result = await query
				.Skip((filter.PageNumber - 1) * filter.PageSize)
				.Take(filter.PageSize)
				.Select(InventoryProjection).ToListAsync();

			return (result, totalCount);
		}
		public async Task<List<InventoryInfo>> GetInventoryByProductAsync(int productId) =>
				 await context.Inventories.Where(i => i.ProductId == productId).Include(e=>e.Products)
								.Select(InventoryProjection).ToListAsync();

		public async Task<List<InventoryInfo>> GetLowStockItemsAsync(int threshold) =>
	               await context.Inventories.Include(e => e.Products).Where(e=>e.QuantityInStock <= threshold)
		                        .Select(InventoryProjection).ToListAsync();

		private static Expression<Func<Inventory, InventoryInfo>> InventoryProjection =>
			inventory => new InventoryInfo
			{
				InventoryId = inventory.InventoryId,
				ProductId = inventory.ProductId,
				ProductName = inventory.Products.Name,
				QuantityInStock = inventory.QuantityInStock,
				WarehouseId = inventory.WarehouseId,
				SerialNumber = inventory.Warehouse.SerialNumber
			};


		public async Task<List<Inventory>> GetInventorysByProductsAndWarehousesAsync(List<int> productId,List<int> warehouseId) =>
			 await context.Inventories.Include(e => e.Products)
			 .Where(e =>  productId.Contains(e.ProductId)  &&warehouseId.Contains(e.WarehouseId) && !e.Products.IsDeleted)
			 .ToListAsync();

	}
}
