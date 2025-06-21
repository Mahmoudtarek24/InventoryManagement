using Application.ResponseDTO_s.AuthenticationResponse;
using Infrastructure.Identity_Models;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.InternalInterfaces
{
	public interface ITokenService
	{
		Task RevokeRefreshTokenAsync(string token);
		Task<SignInResponseDto> RefreshTokenAsync(string token, HttpContext httpContext);
		Task<RefreshToken> SaveRefreshTokenAsync(ApplicationUser user, string token, string ipAddress);
		string GenerateRefreshToken();
		Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user, string[] userRoles);
	}
}
