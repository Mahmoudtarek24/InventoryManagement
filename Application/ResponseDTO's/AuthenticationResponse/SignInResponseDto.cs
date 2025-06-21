using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s.AuthenticationResponse
{
	public class SignInResponseDto
	{
		public string AccessToken { get; set; }
		public DateTime RefreshTokenExpiration { get; set; }
		[JsonIgnore] //will not effect on model state validation as her value nullable , becouse it will not on json generated
		public string RefreshToken { get; set; }
	}
}
