using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.InventoryResponse
{
	public class ProductInventoryResponseDto 
	{
		public List<ProductInventoryResultDto> Items { get; set; }
		public int TotalQuantity { get; set; }
	}
}
