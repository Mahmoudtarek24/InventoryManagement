using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.SupplierDto_s
{
	public class ProductPaginationForSupplierQuery
	{
		const int maxPageSize = 30;
		private int _pageSize = 10;
		public int PageNumber { get; set; }
		public int PageSize
		{
			get { return _pageSize; }
			set
			{
				_pageSize = (value > maxPageSize) ? maxPageSize : value;
			}
		}

		public int TotalCount { get; set; } = 0;
	}
}
