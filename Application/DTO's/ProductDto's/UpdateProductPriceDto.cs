using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.ProductDto_s
{
	public class UpdateProductPriceDto
	{
		public int ProductId { get; set; }
		public decimal NewPrice { get; set; }
	}
}
