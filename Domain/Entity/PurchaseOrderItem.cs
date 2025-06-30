using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
	public class PurchaseOrderItem  
	{
		public int PurchaseOrderItemId { get; set; }	
		public int ProductId { get; set; }		
		public Product Product { get; set; }
		public decimal UnitPrice { get; set; }
		public int OrderQuantity {  get; set; }	
		public int ReceivedQuantity { get; set; }	=0;	
		public int PurchaseOrderId { get; set; }	
		public PurchaseOrder PurchaseOrder { get; set; }

		////relation with Warehouse this Purchase Order where will arrive
 	}
}
