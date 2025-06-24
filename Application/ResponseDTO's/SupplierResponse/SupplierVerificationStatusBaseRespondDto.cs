using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.SupplierResponse
{
	public class SupplierVerificationStatusBaseRespondDto
	{
		public VerificationStatus Status { get; set; }
		public string? Reason { get; set; }
	}
}
