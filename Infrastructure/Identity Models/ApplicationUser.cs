using Infrastructure.Identity_Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string FullName { get; set; } = null!;
		public string ProfileImage { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreateOn { get; set; }
		public DateTime? LastUpdateOn { get; set; }
		public ICollection<RefreshToken> RefreshTokens { get; set; }
	}
}
