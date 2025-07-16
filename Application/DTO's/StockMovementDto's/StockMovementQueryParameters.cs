using Application.Constants.Enum;
using System.Threading.Tasks;

namespace Application.DTO_s.StockMovementDto_s
{
	public class StockMovementQueryParameters :BaseQueryParameters
	{
		public int? WarehouseId { get; set; }
		public int? ProductId { get; set; }
		public MovementType? MovementType { get; set; }
		public DateTime? DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
		public StockMovementOrdering StockMovementOrdering { get; set; }	
	}
}
