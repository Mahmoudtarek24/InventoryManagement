using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.InventoryResponse
{
	public class LowStockAlertDto
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public int QuantityInStock { get; set; }
		public int WarehouseId { get; set; }  // اختياري لو عندك أكتر من مخزن
		public string SerialNumber { get; set; }
	}
}
