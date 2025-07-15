using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.PurchaseOrder
{
	public class AddOrderItemDto
	{
		public int? ProductId { get; set; }
		public int? OrderQuantity { get; set; }
		public List<OrderItemDto>? Items { get; set; }
	}
}
