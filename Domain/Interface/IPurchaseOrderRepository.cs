﻿
using Domain.Entity;
using Domain.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
	public interface IPurchaseOrderRepository :IGenaricRepository<PurchaseOrder>
	{
		Task<PurchaseOrder?> GetPurchaseOrderWithItemsAsync(int purchaseOrderId);
		Task<PurchaseOrder?> GetPurchaseOrderWithItemsAndSupplierAsync(int purchaseOrderId);
		Task<(int, List<PurchaseOrder>)> GetPurchaseOrdersWithFiltersAsync(PurchaseOrderFilter query);
	}
}
