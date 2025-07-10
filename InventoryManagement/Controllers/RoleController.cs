using Application.Constants;
using Application.DTO_s.RolesDto_s;
using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Admin)]
	public class RoleController : ControllerBase
	{
		private readonly IRoleService roleService;
		public RoleController(IRoleService roleService)
		{
			this.roleService = roleService;
		}
		[HttpGet]
		public async Task<IActionResult> GetRoles()
		{
			var result = await roleService.GetRoleListAsync();
			return Ok(result);
		}
		[HttpPut("user")]
		public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesDto dto)
		{
			var result = await roleService.UpdateUserRolesAsync(dto);
			return Ok(result);
		}
	}
}
