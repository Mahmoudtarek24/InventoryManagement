using Application.Constants;
using Application.DTO_s;
using Application.DTO_s.AuthenticationDto_s;
using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Admin)]
	public class ApplicationUserController : ControllerBase
	{
		private readonly IUserService userService;
		private readonly IUserContextService userContextService;
		public ApplicationUserController(IUserService userService,IUserContextService userContextService)
		{
			this.userService = userService;	
			this.userContextService = userContextService;	
		}

		[HttpGet("by-ID")] /// =>[HttpGet("{userId}")]=> with path parameter shoud sed user id , with query parameter can not send it
		public async Task<IActionResult> GetUserById([FromQuery] string? userId)
		{
			if (string.IsNullOrEmpty(userId))
				userId = userContextService.userId;
			else
			{
				if (!userContextService.IsAdmin && userId != userContextService.userId)
					return Forbid();
			}
			var result = await userService.FindByIdAsync(userId);
			return StatusCode(result.StatusCode,result);
		}

		[HttpGet("by-email")]
		public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
		{
			var result = await userService.FindByEmailAsync(email);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("paginated")]
		public async Task<IActionResult> GetUsersPaged([FromQuery] ApplicationUserQueryParameters query)
		{
			var route = $"{Request.Path}";
			var result = await userService.GetUsersWithPaginationAsync(query, route);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPatch("unlock/{userId}")]
		public async Task<IActionResult> UnlockUser(string userId)
		{
			var result = await userService.UnLOckedUsers(userId);
			return StatusCode(result.StatusCode, result);
		}

		[HttpDelete("{userId}")]
		public async Task<IActionResult> SoftDeleteUser(string userId)
		{
			var result = await userService.SoftDeleteUserAsync(userId);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPut("profile/{userId}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer)]
		public async Task<IActionResult> UpdateUserProfile([FromRoute] string userId, [FromForm] UpdateUserProfileDto dto)
		{
			if (userId != dto.UserId)
				return BadRequest("Route UserId not equal userId on object");

			var result = await userService.UpdateProfileAsync(dto);
			return StatusCode(result.StatusCode, result);
		}
	}
}
