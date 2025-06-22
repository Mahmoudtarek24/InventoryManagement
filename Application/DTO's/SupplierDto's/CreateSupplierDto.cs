using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.SupplierDto_s
{
	public class CreateSupplierDto
	{
		public string CompanyName { get; set; }
		public string Address { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }	
		public string Email { get; set; }	
		public string PhoneNumber { get; set; }
	}
}
