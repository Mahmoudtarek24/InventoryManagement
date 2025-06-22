using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
	public class Supplier : BaseModel   ///business Entity execute on it
	{
		public int SupplierId { get; set; }	
		public string CompanyName { get; set; }	
		public string Address { get; set; }
		public string? Notes { get; set; }
		public string? TaxDocumentPath { get; set; }
		public bool IsVerified { get; set; }
		public string UserId { get; set; }
		public ICollection<Product> Products { get; set; }		  
	}
}
/////مش هقدر اتعامل معاه لا لما اطب انه يعمل verification