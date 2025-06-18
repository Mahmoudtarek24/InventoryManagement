using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.ProductResponse
{
	public class ProductWithCategoryRespondDto :ProductBaseRespondDto
	{
		public string CategoryName { get; set; }
		//public int? StockQuantity { get; set; }
	}
}
