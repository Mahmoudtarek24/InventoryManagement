using Application.Constants.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.SupplierDto_s
{
	public class ChangeSupplierVerificationStatusDto
	{
		public int SupplierId { get; set; }	
		public VerificationStatus newStatus { get; set; }		
		public string? RejectionReason { get; set; }	///fluent validation to chech is executed when is rejected
	}
}
