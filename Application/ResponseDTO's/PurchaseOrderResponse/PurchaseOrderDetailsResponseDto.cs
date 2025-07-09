using Application.DTO_s.PurchaseOrder;
using Application.ResponseDTO_s.SupplierResponse;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.PurchaseOrder
{
	public class PurchaseOrderDetailsResponseDto
	{
		public int PurchaseOrderId { get; set; }
		public DateTime ExpectedDeliveryDate { get; set; }
		public decimal TotalCost { get; set; }
		public PurchaseOrderStatus PurchaseOrderStatus { get; set; }
		public SupplierBaseResponsDto Supplier { get; set; }
		public List<PurchaseOrderItemResponseDto> OrderItems { get; set; }
		public DateTime CreatedOn { get; set; }
		//public string CreatedBy { get; set; }
	}
}
