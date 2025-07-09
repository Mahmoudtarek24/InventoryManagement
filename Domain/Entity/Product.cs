using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
	public class Product :BaseModel  //business entity but on domain take execution on it 
	{
		public int ProductId { get; set; }	
		public string Name { get; set; }	
		public string Barcode { get; set; }	
		public decimal Price { get; set; }	
		public bool IsAvailable { get; set; }	
		public int CategoryId { get; set; }
		public Category Category { get; set; }
		public int SupplierId { get; set; }	
		public Supplier Supplier { get; set; }
		public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
		public ICollection<Inventory> Inventories { get; set; }
	    public ICollection<StockMovement> StockMovements { get; set; }
	}
}

/// domain entity represent real entity on our business based on it can take execution like add , update ,delete 
/// domain order to have id but view can not have id  
