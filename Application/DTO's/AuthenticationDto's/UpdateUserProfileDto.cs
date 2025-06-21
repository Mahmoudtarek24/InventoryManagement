using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.AuthenticationDto_s
{
	public class UpdateUserProfileDto
	{
        public string UserId { get; set; }	
		public string UserName { get; set; }
		public string Email { get; set; }	
		public IFormFile? ImageFile { get; set; }	
		public string FullName { get; set; }	
	}
}
