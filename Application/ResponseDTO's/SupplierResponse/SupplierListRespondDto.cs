using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.SupplierResponse
{
	public class SupplierListRespondDto : SupplierBaseResponsDto
	{
		public int ProductCount { get; set; }
		public string Email { get; set; }	
		public string PhoneNumber { get; set; }	
		public string UserId { get; set; }
	}
}
