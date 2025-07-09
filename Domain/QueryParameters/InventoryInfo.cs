using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.QueryParameters
{
	public class InventoryInfo
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public int QuantityInStock { get; set; }
		public int WarehouseId { get; set; }
		public string SerialNumber { get; set; }
		public int InventoryId { get; set; }
	}
}
