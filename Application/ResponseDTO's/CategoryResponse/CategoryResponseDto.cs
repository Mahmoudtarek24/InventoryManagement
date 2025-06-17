using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.CategoryResponse
{
	public class CategoryResponseDto
	{
		public int CategoryId { get; set; }	
		public string Name { get; set; }
		public string? Description { get; set; }
		public int DisplayOrder { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreateOn { get; set; }
		public DateTime? LastUpdateOn { get; set; }
	}
}
