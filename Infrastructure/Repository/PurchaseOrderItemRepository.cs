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
	public class PurchaseOrderItemRepository : GenaricRepository<PurchaseOrderItem> ,IPurchaseOrderItemRepository
	{
		public PurchaseOrderItemRepository(InventoryManagementDbContext context) : base(context) { }

		public async Task<List<PurchaseOrderItem>> GetPurchaseHistoryByProductIdAsync(int productId) =>
				     await context.PurchaseOrderItems.AsNoTracking().Include(poi => poi.PurchaseOrder)
			         .ThenInclude(po => po.Supplier)
			         .Where(poi => poi.ProductId == productId && !poi.PurchaseOrder.IsDeleted)
			         .OrderByDescending(poi => poi.PurchaseOrder.CreateOn).ToListAsync();

	}
}
