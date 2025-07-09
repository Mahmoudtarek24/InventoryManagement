using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
	public class Inventory  ////مخزون اتلبضاعه في مخزن معين
	{
		public int InventoryId { get; set; }
		public int ProductId { get; set; }
		public Product Products { get; set; }
		public int WarehouseId { get; set; }
		public Warehouse Warehouse { get; set; }
		public int QuantityInStock { get; set; }
	}
}
