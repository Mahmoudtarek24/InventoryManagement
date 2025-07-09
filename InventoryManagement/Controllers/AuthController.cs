using Application.Constants;
using Application.DTO_s.AuthenticationDto_s;
using Application.Interfaces;
using Infrastructure.InternalInterfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService authService ;
		public AuthController(IAuthService authService)
		{
			this.authService = authService ;	
		}

		[HttpPost("register")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Admin)]
		public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
		{
			var result = await authService.CreateUserAsync(dto);
			return Ok(result);
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] SignInDto dto)
		{
			var result = await authService.SignInAsync(dto);
			
			if (result.IsSuccess && result.Data != null && !string.IsNullOrEmpty(result.Data.RefreshToken))
			{
				SetRefreshTokenCookie(result.Data.RefreshToken, result.Data.RefreshTokenExpiration);
			}
			return Ok(result);
		}

		[HttpPost("refresh-token")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer)]
		public async Task<IActionResult> RefreshToken()
		{
			var refreshToken = Request.Cookies["refreshToken"];
			if (string.IsNullOrEmpty(refreshToken)) 
				return BadRequest(new { message = "Refresh token is required" });

			var result = await authService.RefreshTokenAsync(refreshToken, HttpContext);

			return Ok(result);
		}

		[HttpPost("revoke-token")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer)]
		public async Task<IActionResult> RevokeToken()
		{
			var refreshToken = Request.Cookies["refreshToken"];
			if (string.IsNullOrEmpty(refreshToken))
				return BadRequest(new { message = "Refresh token is required" });

			await authService.RevokeTokenAsync(refreshToken);
			Response.Cookies.Delete("refreshToken");
		
			return Ok(new { message = "Token revoked successfully" });
		}
		private void SetRefreshTokenCookie(string refreshToken, DateTime expires)
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				IsEssential = true,
				Secure = false,
				Expires = expires.ToLocalTime(),
			};
			Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
		}
	}
}
