using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity_Models
{
	[Owned]
	public class RefreshToken
	{
		public string Token { get; set; }	
		public DateTime Expires { get; set; }	
		public DateTime? Revoked { get; set; }	
		public DateTime CreateOn { get; set; }
		public string CreatedByIp { get; set; }
		public bool IsActive =>Revoked is null&& !IsExpired ;
		public bool IsExpired =>DateTime.UtcNow >= Expires;	
	}
}
