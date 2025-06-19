﻿using Microsoft.AspNetCore.Identity;
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
	}
}
