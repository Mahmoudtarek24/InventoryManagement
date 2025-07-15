using Domain.Entity;
using Domain.Interface;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
	public class StockMovementRepository :GenaricRepository<StockMovement> , IStockMovementRepository 
	{
		public StockMovementRepository(InventoryManagementDbContext context) : base(context) { }
		public async Task<List<StockMovement>> GetStockMovementsByProductAsync(int productId, int pageNumber) =>
				 await context.StockMovements.Include(sm => sm.Product).ThenInclude(e=>e.PurchaseOrderItems)
				 .Include(sm => sm.SourceWarehouse).Include(sm => sm.DestinationWarehouse)
				 .Where(sm => sm.prodcutId == productId).OrderByDescending(sm => sm.MovementDate)
				 .Skip((pageNumber-1)*20).Take(20).ToListAsync();

		public async Task<List<StockMovement>> GetStockMovementsByWarehouseAsync(int warehouseId, int pageNumber) =>
			 await context.StockMovements.Include(sm => sm.Product).ThenInclude(e => e.PurchaseOrderItems)
					 .Include(sm => sm.SourceWarehouse).Include(sm => sm.DestinationWarehouse)
					 .Where(sm => sm.SourceWarehouseId == warehouseId||sm.DestinationWarehouseId==warehouseId)
			         .OrderByDescending(sm => sm.MovementDate).Skip((pageNumber - 1) * 20).Take(20).ToListAsync();
	}
}
