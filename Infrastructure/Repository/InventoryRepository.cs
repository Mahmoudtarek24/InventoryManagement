using Application.ResponseDTO_s.InventoryResponse;
using Domain.Entity;
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

		public async Task<InventoryInfo?> GetInventoryByProductAndWarehouseAsync(int productId, int warehouseId)
		{
			return await context.Inventories
				 .Where(i => i.ProductId == productId && i.WarehouseId == warehouseId && !i.Products.IsDeleted)
				.Select(InventoryProjection).FirstOrDefaultAsync();
		}

		public async Task<(List<InventoryInfo>, int)> GetInventoryByWarehouseWithFiltersAsync
																				   (int warehouseId, BaseFilter filter)
		{
			var query = context.Inventories
				.Where(i => i.WarehouseId == warehouseId &&!i.Products.IsDeleted ).AsQueryable();

			if (!string.IsNullOrEmpty(filter.searchTearm))
			{
				query = query.Where(i =>
					i.Products.Name.Contains(filter.searchTearm, StringComparison.OrdinalIgnoreCase));
			}

			int totalCount = await query.CountAsync();
	
			query = query.OrderBy(i => i.Products.Name);

			var result = await query
				.Skip((filter.PageNumber - 1) * filter.PageSize)
				.Take(filter.PageSize)
				.Select(InventoryProjection).ToListAsync();

			return (result, totalCount);
		}
		public async Task<List<InventoryInfo>> GetInventoryByProductAsync(int productId) =>
				 await context.Inventories.Where(i => i.ProductId == productId)
								.Select(InventoryProjection).ToListAsync();

		public async Task<List<InventoryInfo>> GetLowStockItemsAsync(int threshold) =>
	               await context.Inventories.Where(e=>e.QuantityInStock <= threshold)
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

	}
}
