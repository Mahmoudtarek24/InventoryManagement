using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.QueryParameters
{
	public class SupplierInfo
	{
		public int SupplierId { get; set; }
		public string CompanyName { get; set; }
		public string Address { get; set; }
		public string? Notes { get; set; }
		public string? TaxDocumentPath { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime? LastUpdateOn { get; set; }
		public DateTime CreateOn { get; set; }
		public bool IsVerified { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string userId { get; set; }
		public bool EmailConfirmed { get; set; }
		public int ProductCount { get; set; }
	}
}
