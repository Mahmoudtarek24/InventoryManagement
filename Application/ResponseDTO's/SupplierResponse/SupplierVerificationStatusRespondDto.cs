using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.SupplierResponse
{
	public class SupplierVerificationStatusRespondDto :SupplierVerificationStatusBaseRespondDto
	{
		public int SupplierId { get; set; }
		public string CompanyName { get; set; }
		public string Address { get; set; }
		public bool IsVerified { get; set; }
		
	}
}
