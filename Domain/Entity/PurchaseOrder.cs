using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
	public class PurchaseOrder :BaseModel 
	{
		public int PurchaseOrderId { get; set; }
		public DateTime ExpectedDeliveryDate { get; set; }
		public decimal TotalCost { get; set; }	
		public int SupplierId { get; set; }
		public Supplier Supplier { get; set; }
		public PurchaseOrderStatus PurchaseOrderStatus { get; set; }
		public ICollection<PurchaseOrderItem> OrderItems { get; set; }
		public int WarehouseId { get; set; }
		public Warehouse Warehouse { get; set; }	
	}
}
