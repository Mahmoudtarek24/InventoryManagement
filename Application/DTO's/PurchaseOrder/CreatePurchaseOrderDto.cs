using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.PurchaseOrder
{
	public class CreatePurchaseOrderDto
	{
		public int SupplierId { get; set; }
		public DateTime ExpectedDeliveryDate { get; set; }
		public PurchaseOrderStatus purchaseOrderStatus { get; set; }	
		public List<PurchaseOrderItemDto> purchaseOrderItemDtos { get; set; }	= new List<PurchaseOrderItemDto>();	   
	}
}
