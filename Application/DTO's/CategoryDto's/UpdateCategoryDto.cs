using Application.DTO_s.CategoryDto_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s
{
	public class UpdateCategoryDto : CategoryBaseDto
	{
		public int CategoryId { get; set; }	
	}
}
