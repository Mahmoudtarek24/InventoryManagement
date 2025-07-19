using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.StockMovementDto_s
{
	public class RecordStockMovementDto
	{
		public int WarehouseId { get; set; }
		public List<ProductsMovment> ProductsMovments { get; set; }
	}
}
