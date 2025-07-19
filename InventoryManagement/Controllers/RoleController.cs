using Application.Constants;
using Application.DTO_s.RolesDto_s;
using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
		[SwaggerOperation( Summary = "Get roles for dropdown",
	    Description = "Returns a list of all roles to be used in dropdowns. Only accessible by Admin." )]
		public async Task<IActionResult> GetRoles()
		{
			var result = await roleService.GetRoleListAsync();
			return StatusCode(result.StatusCode,result);
		}
		[HttpPut("user")]
		[SwaggerOperation( Summary = "Add or remove user from roles ")]
		public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesDto dto)
		{
			var result = await roleService.UpdateUserRolesAsync(dto);
			return StatusCode(result.StatusCode, result);
		}
	}
}
