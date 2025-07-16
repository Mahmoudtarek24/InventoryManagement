using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s
{
	public class BaseQueryParameters: PaginationQueryParameters
	{	
		public string? searchTearm { get; set; }
		public string? SortBy { get; set; }
		public bool SortAscending { get; set; } = true;
		
	}
}
