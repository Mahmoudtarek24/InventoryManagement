using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.ProductResponse
{
	public class ProductsBySupplierResponseDto : ProductBaseRespondDto
	{
		public int SupplierId { get; set; }
		public string CompanyName { get; set; }
		public string Address { get; set; }
		public string Notes { get; set; }
		public bool IsVerified { get; set; }
		public VerificationStatus VerificationStatus { get; set; }
	}
}
