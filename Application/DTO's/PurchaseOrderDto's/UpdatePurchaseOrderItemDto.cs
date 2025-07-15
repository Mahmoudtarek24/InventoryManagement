using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.PurchaseOrder
{
	public class UpdatePurchaseOrderItemDto
	{
		public int PurchaseOrderItemId { get; set; }
		public int? OrderQuantity { get; set; }
		public int? ProductId { get; set; }	
	}
}
