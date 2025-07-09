using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.ProductResponse
{
	public class ProductBaseRespondDto
	{
		public int ProductId { get; set; }
		public string Name { get; set; }
		public string Barcode { get; set; }
		public decimal Price { get; set; }
		public bool IsAvailable { get; set; }
		public int CategoryId { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreateOn { get; set; }
		public DateTime? LastUpdateOn { get; set; }
	}
}