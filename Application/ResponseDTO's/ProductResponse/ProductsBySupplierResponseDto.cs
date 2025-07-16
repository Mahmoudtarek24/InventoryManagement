using Application.Constants.Enum;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.ProductResponse
{
	public class ProductsBySupplierResponseDto 
	{
		public int SupplierId { get; set; }
		public string CompanyName { get; set; }
		public string Address { get; set; }
		public string Notes { get; set; }
		public bool IsVerified { get; set; } 
		public VerificationStats VerificationStatus { get; set; }
		public PagedResponse<List<ProductBaseRespondDto>> SupplierProducts { get; set; }	 
	}
}
