using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.PurchaseOrder
{
	public class PurchaseOrderItemResponseDto 
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public decimal UnitPrice { get; set; }
		public int OrderQuantity { get; set; }
		public int ReceivedQuantity { get; set; }
	}
}
