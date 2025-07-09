using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
	public class StockMovement
	{
		public int StockMovementId { get; set; }	 
		public StockMovementType MovementType { get; set; }	
		public DateTime MovementDate {  get; set; }	
		public int prodcutId { get; set; }	
		public Product Product { get; set; }
		public int Quantity {  get; set; }
		public int? SourceWarehouseId { get; set; }
		public Warehouse SourceWarehouse { get; set; }
		public int? DestinationWarehouseId { get; set; }
		public Warehouse DestinationWarehouse { get; set; }
	}
}
/// الحالة                    SourceWarehouseId 	       DestinationWarehouseId
/// استلام من المورد	               null	                         Cairo Warehouse
/// تحويل من مخزن لمخزن	       Warehouse A	                       Warehouse B
/// إرجاع                       Warehouse A	                          null






