using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.InventoryResponse
{
	public class ProductInventoryResultDto
	{
		public int QuantityInStock { get; set; }
		public int WarehouseId { get; set; }
		public string SerialNumber { get; set; }
		public int InventoryId { get; set; }
	}
}
