using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.ProductDto_s
{
	public class ProductQueryParameters
	{
		const int maxPageSize = 30;
		private int _pageSize = 10;

		public string searchTearm { get; set; }
		public string SortBy { get; set; }
		public bool SortAscending { get; set; } = true;
		public int PageNumber { get; set; } = 1;
		public int PageSize
		{
			get { return _pageSize; }
			set
			{
				_pageSize = (value > maxPageSize) ? maxPageSize : value;
			}
		}
	}
}
