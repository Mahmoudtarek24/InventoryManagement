using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.AuthenticationResponse
{
	public class SignInResponseDto
	{
		public string Token { get; set; }
		public DateTime Expiration { get; set; }
		public string UserId { get; set; }
		public string Email { get; set; }
	}
}
