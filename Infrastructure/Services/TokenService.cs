using Application.ResponseDTO_s.AuthenticationResponse;
using Domain.Interface;
using Infrastructure.Enum;
using Infrastructure.Identity_Models;
using Infrastructure.InternalInterfaces;
using Infrastructure.Models;
using Infrastructure.settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
	public class TokenService : ITokenService
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly JWTSetting jWTSetting;
		private readonly IUnitOfWork unitOfWork;	
		public TokenService(UserManager<ApplicationUser> UserManager, RoleManager<IdentityRole> roleManager
						  , IOptions<JWTSetting> options, IUnitOfWork unitOfWork)
		{
			this.userManager = UserManager;
			this.roleManager = roleManager;
			this.jWTSetting = options.Value;
			this.unitOfWork = unitOfWork;
		}
		public async Task<SignInResponseDto> RefreshTokenAsync(string token, HttpContext httpContext) //copy past
		{
			// will serch for refresh token on table 
			var userByToken = await userManager.Users.SingleOrDefaultAsync(e => e.RefreshTokens.Any(e => e.Token == token));

			var oldRefreshToken = userByToken.RefreshTokens.FirstOrDefault(e => e.IsActive);

			if (userByToken is null || oldRefreshToken is null)
				throw new SecurityTokenException("Invalid refresh token");


			oldRefreshToken.Revoked = DateTime.Now;
			var newRefreshToken = GenerateRefreshToken();
			var refreshToken = await SaveRefreshTokenAsync(userByToken, newRefreshToken, "12345");

			SetRefreshTokenCookie(httpContext.Response, newRefreshToken);

			var Roles = await userManager.GetRolesAsync(userByToken);
			var newAccessToken = await GenerateJwtToken(userByToken, Roles.ToArray());

			return new SignInResponseDto
			{
				AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
				RefreshTokenExpiration = refreshToken.Expires,
			};
		}

		public string GenerateRefreshToken()
		{
			var randomBytes = new byte[64];
			using var generator = new RNGCryptoServiceProvider();
			generator.GetBytes(randomBytes);
			return Convert.ToBase64String(randomBytes);
		}

		public async Task RevokeRefreshTokenAsync(string token)
		{
			var userByToken = await userManager.Users.SingleOrDefaultAsync(e => e.RefreshTokens.Any(e => e.Token == token));

			var RefreshToken = userByToken.RefreshTokens.FirstOrDefault(e => e.IsActive);

			if (userByToken is null || RefreshToken is null)
				return;

			RefreshToken.Revoked = DateTime.Now;
			await userManager.UpdateAsync(userByToken);
		} /////need to delete cookies on end point 

		public async Task<RefreshToken> SaveRefreshTokenAsync(ApplicationUser user, string token, string ipAddress)
		{
			var refreshToken = new RefreshToken
			{
				CreatedByIp = ipAddress,
				CreateOn = DateTime.Now,
				Expires = DateTime.Now.AddDays(jWTSetting.RefreshTokenExpirationInDays),
				Token = GenerateRefreshToken(),
			};
			user.RefreshTokens.Add(refreshToken);
			await userManager.UpdateAsync(user);
			return refreshToken;
		}
		private void SetRefreshTokenCookie(HttpResponse response, string refreshToken)  /// i will execut when we execute RefreshTokenAsync/
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				IsEssential = true,
				Secure = true,
				Expires = DateTime.Now.AddDays(jWTSetting.RefreshTokenExpirationInDays).ToLocalTime(),
			};
			response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
		}
		public async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user, string[] userRoles)
		{
			List<Claim> claims = new List<Claim>();

			if (userRoles.Contains($"{AppRoles.Supplier}"))
			{
				var SupplierVerification =await unitOfWork.SupplierRepository.IsVerifiedAndActiveSupplierAsync(user.Id);
				claims.Add(new Claim("IsSupplierVerified", SupplierVerification.ToString()));
			}

			if (userRoles.Length > 0)
				foreach (var role in userRoles)
					claims.Add(new Claim(ClaimTypes.Role, role));
			

			claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
			claims.Add(new Claim(ClaimTypes.Email, user.Email));
			claims.Add(new Claim(ClaimTypes.Name, user.UserName));
			claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

			SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWTSetting.Key));
			SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
			return new JwtSecurityToken(
				claims: claims,
				issuer: jWTSetting.Issuer,
				audience: jWTSetting.Audience,
				expires: DateTime.Now.AddDays(jWTSetting.DurationInDays),
				signingCredentials: signingCredentials);
		}
	}
}
