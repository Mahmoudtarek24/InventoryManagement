using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
	public class Warehouse   //like cairo Warehouse , alex ......
	{
		public int WarehouseId { get; set; }
		public string SerialNumber { get; set; }	
		public ICollection<Inventory> Inventories { get; set; }
		public ICollection<PurchaseOrder> purchaseOrders { get; set; }

		public ICollection<StockMovement> SourceStockMovements { get; set; }
		public ICollection<StockMovement> DestinationStockMovements { get; set; }
	}
}

/// when i have two foreign key on another domain i should put 2 navigation properties  
/// or set it without any navighation properties 