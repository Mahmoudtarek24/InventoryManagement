using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.SupplierResponse
{
	public class SupplierBaseResponsDto
	{ 
		public int SupplierId { get; set; }
		public string CompanyName { get; set; }
		public string Address { get; set; }
		public bool IsVerified { get; set; }
		public DateTime CreateOn { get; set; }
		public DateTime? LastUpdateOn { get; set; }
		public bool IsDeleted { get; set; }
	}
}
