using Application.DTO_s.AuthenticationDto_s;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.AuthenticationResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface IAuthService //authentication 
	{
		Task<ApiResponse<AuthenticationResponseDto>> CreateUserAsync(CreateUserDto dto);
		Task<ApiResponse<SignInResponseDto>> SignInAsync(SignInDto SignInDto);
		//Task SignOutAsync();
	    //refresh token 
		//validate token
	}
}
