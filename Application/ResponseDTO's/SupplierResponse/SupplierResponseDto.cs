using Application.ResponseDTO_s.ProductResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.SupplierResponse
{
	public class SupplierResponseDto : SupplierBaseResponsDto
	{
		public string? Notes { get; set; }
		public string? TaxDocumentPath { get; set; }
		public string UserId { get; set; }
		public PagedResponse<IEnumerable<ProductBaseRespondDto>> Products { get; set; }
	}
}
