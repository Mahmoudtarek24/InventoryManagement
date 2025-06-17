using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
	public class BaseModel
	{
		public bool IsDeleted { get; set; }	
		public DateTime CreateOn { get; set; }	
		public DateTime? LastUpdateOn { get; set; }	
	}
}
