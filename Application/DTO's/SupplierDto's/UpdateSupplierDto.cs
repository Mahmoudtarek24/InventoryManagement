using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.SupplierDto_s
{
	public class UpdateSupplierDto
	{
		public string CompanyName { get; set; }
		public string Address { get; set; }
		public string? Notes { get; set; }              // Admin only
		public bool? IsVerified { get; set; }
	}
}
