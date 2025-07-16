using Application.Constants.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.ProductDto_s
{
	public class SupplierProductsQueryParameters : BaseQueryParameters
	{
		public ProductOrdering SortBys { get; set; }
	}
}
