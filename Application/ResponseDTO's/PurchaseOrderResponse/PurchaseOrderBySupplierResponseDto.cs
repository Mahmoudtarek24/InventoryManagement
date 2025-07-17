using Application.Constants.Enum;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.PurchaseOrder
{
	public class PurchaseOrderBySupplierResponseDto
	{
		public int PurchaseOrderId { get; set; }
		public DateTime ExpectedDeliveryDate { get; set; }
		public decimal TotalCost { get; set; }
		public PurchaseStatus PurchaseOrderStatus { get; set; }
		public int NumberOfItems { get; set; }
	}
}
