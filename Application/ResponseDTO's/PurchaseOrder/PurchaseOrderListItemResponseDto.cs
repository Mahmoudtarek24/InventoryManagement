using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.PurchaseOrder
{
	public class PurchaseOrderListItemResponseDto
	{
		public int PurchaseOrderId { get; set; }
		public string SupplierName { get; set; }
		public PurchaseOrderStatus Status { get; set; }
		public DateTime ExpectedDeliveryDate { get; set; }
		public decimal TotalCost { get; set; }
		public int TotalItems { get; set; }
		public DateTime CreatedOn { get; set; }
	}
}
