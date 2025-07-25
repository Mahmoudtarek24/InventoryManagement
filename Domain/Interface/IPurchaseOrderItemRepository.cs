﻿using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
	public interface IPurchaseOrderItemRepository :IGenaricRepository<PurchaseOrderItem>
	{
		Task<List<PurchaseOrderItem>> GetPurchaseHistoryByProductIdAsync(int productId);
	}
}
