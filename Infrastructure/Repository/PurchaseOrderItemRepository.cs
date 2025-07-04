﻿using Domain.Entity;
using Domain.Interface;
using Infrastructure.Context;
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

	}
}
