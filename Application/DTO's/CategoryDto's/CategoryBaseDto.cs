using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.CategoryDto_s
{
	public class CategoryBaseDto
	{
		public string Name { get; set; }
		public string? Description { get; set; }
		public int DisplayOrder { get; set; }
	}
}
