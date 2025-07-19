using Application.Constants.Enum;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.StockMovementResponse
{
	public class StockMovementResponseDto
	{
		public DateTime MovementDate { get; set; }
		public MovementType MovementType { get; set; }
		public int Quantity { get; set; }

		public int ProductId { get; set; }	
		public string ProductName { get; set; } 
		public string? WarehouseName { get; set; }

		// Purchase 
		public decimal? UnitPrice { get; set; }
		public decimal? TotalCost => UnitPrice.HasValue ? UnitPrice * Quantity : null;

		// Transfer 
		public string? SourceWarehouseName { get; set; }
		public string? DestinationWarehouseName { get; set; }

	}
}
