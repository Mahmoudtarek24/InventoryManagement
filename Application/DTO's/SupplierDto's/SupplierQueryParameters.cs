using Application.Constants.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.SupplierDto_s
{
	public class SupplierQueryParameters :BaseQueryParameters
	{
		public SuppliersOrdering SortOption { get; set; }
	}
}
