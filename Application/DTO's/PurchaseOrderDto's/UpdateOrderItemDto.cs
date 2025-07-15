using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.PurchaseOrderDto_s
{
	public class UpdateOrderItemDto
	{
		public int ProductId { get; set; }
		public int? OrderQuantity { get; set; }
	}
}
