using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.PurchaseOrder
{
	public class UpdatePurchaseOrderDto
	{
		public int PurchaseOrderId {  get; set; }	
		public DateTime ExpectedDeliveryDate { get; set; }   /////will validate on fluent api 
		public PurchaseOrderStatus? purchaseOrderStatus { get; set; }
		public List<UpdatePurchaseOrderItemDto>? purchaseOrderItemDtos { get; set; } = new List<UpdatePurchaseOrderItemDto>();
	}
}
