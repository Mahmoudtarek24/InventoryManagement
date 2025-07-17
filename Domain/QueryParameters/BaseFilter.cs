using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Parameters
{
	public class BaseFilter
	{
		public string? searchTearm { get; set; }
		public string SortBy { get; set; }
		public bool SortAscending { get; set; }
		public int PageNumber { get; set; } 
		public int PageSize { get; set; }
	}
}
