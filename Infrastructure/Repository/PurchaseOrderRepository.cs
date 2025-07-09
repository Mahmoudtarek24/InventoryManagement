using Application.Constants.Enum;
using Application.DTO_s.PurchaseOrder;
using Application.ResponseDTO_s.PurchaseOrder;
using Domain.Entity;
using Domain.Interface;
using Domain.Parameters;
using Domain.QueryParameters;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
	public class PurchaseOrderRepository :GenaricRepository<PurchaseOrder> ,IPurchaseOrderRepository
	{
		public PurchaseOrderRepository(InventoryManagementDbContext context) : base(context) { }

		public async Task<PurchaseOrder?> GetPurchaseOrderWithItemsAsync(int purchaseOrderId) =>
			            await context.PurchaseOrders.Include(e => e.OrderItems)
				               .SingleOrDefaultAsync(e => e.PurchaseOrderId == purchaseOrderId);

		public async Task<PurchaseOrder?> GetPurchaseOrderWithItemsAndSupplierAsync(int purchaseOrderId) =>
		    	 await context.PurchaseOrders.AsNoTracking().Include(po => po.Supplier).Include(po => po.OrderItems)
			    .ThenInclude(oi => oi.Product)
			    .FirstOrDefaultAsync(po => po.PurchaseOrderId == purchaseOrderId && !po.IsDeleted);

		public async Task<(int, List<PurchaseOrder>)> GetPurchaseOrdersWithFiltersAsync(PurchaseOrderFilter query)
		{
			var purchaseOrderQuery = context.PurchaseOrders
				.AsNoTracking()
				.Include(po => po.Supplier)
				.Include(po => po.OrderItems)
				.Where(po => !po.IsDeleted).AsQueryable();

			if (query.Status.HasValue)
				purchaseOrderQuery = purchaseOrderQuery.Where(po => po.PurchaseOrderStatus == query.Status.Value);

			if (!string.IsNullOrEmpty(query.searchTearm))
				purchaseOrderQuery = purchaseOrderQuery.Where(po =>
					po.Supplier.CompanyName.Contains(query.searchTearm, StringComparison.OrdinalIgnoreCase) ||
					po.PurchaseOrderId.ToString().Contains(query.searchTearm));

			int totalCount = await purchaseOrderQuery.CountAsync();

			switch (query.SortBy.ToLower())
			{
				case "companyname":
					purchaseOrderQuery = query.SortAscending
						? purchaseOrderQuery.OrderBy(po => po.Supplier.CompanyName)
						: purchaseOrderQuery.OrderByDescending(po => po.Supplier.CompanyName);
					break;
				case "totalcost":
					purchaseOrderQuery = query.SortAscending
						? purchaseOrderQuery.OrderBy(po => po.TotalCost)
						: purchaseOrderQuery.OrderByDescending(po => po.TotalCost);
					break;
				default:
					purchaseOrderQuery = query.SortAscending
						? purchaseOrderQuery.OrderBy(po => po.CreateOn)
						: purchaseOrderQuery.OrderByDescending(po => po.CreateOn);
					break;
			}

			var result = await purchaseOrderQuery
				.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToListAsync();

			return (totalCount, result);
		}
		public async Task<List<PurchaseOrder>> GetPurchaseOrdersBySupplierAsync(int supplierId) =>
			      await context.PurchaseOrders.AsNoTracking().Include(po => po.OrderItems)
							   .Where(po => po.SupplierId == supplierId && !po.IsDeleted)
							   .OrderByDescending(po => po.CreateOn).ToListAsync();

	}
}
