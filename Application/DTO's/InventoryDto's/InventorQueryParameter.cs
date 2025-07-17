using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.InventoryDto_s
{
	public class InventorQueryParameter :PaginationQueryParameters
	{
		public string? searchTearm {  get; set; }	
	}
}
