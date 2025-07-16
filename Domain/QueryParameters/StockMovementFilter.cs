using Domain.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.QueryParameters
{
	public class StockMovementFilter : BaseFilter
	{
		public int? WarehouseId { get; set; }
		public int? ProductId { get; set; }
		public string? MovementType { get; set; }
		public DateTime? DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
		public string? StockMovementOrdering { get; set; }
	}
}
