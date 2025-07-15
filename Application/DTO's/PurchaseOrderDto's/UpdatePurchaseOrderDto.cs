using Application.Constants.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.PurchaseOrder
{
	public class UpdatePurchaseOrderDto
	{
		public DateTime? ExpectedDeliveryDate { get; set; }
		public PurchaseStatus? PurchaseOrderStatus { get; set; }
		public int? WarehouseId { get; set; }
	}
}
