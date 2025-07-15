using Application.DTO_s.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.PurchaseOrderDto_s
{
	public class ValidatePurchaseItems
	{
		public List<OrderItemDto> ValidItems { get; set; } = new List<OrderItemDto>();
		public List<string> Warnings { get; set; } = new List<string>();
		public List<OrderItemDto> ItemsToUpdateQuantity { get; set; }	= new List<OrderItemDto>();	
	}
}
