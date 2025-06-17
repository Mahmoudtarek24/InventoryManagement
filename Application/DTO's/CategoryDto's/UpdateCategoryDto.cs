using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s
{
	public class UpdateCategoryDto
	{
		public int CategoryId { get; set; }	
		public string Name { get; set; }
		public string? Description { get; set; }
		public int DisplayOrder { get; set; }
	}
}
