using Application.Constants;
using Application.DTO_s.SupplierDto_s;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SupplierController : ControllerBase
	{
		private readonly ISupplierServices supplierService;
		private readonly IUserContextService userContextService;
		public SupplierController(ISupplierServices supplierServices, IUserContextService userContextService)
		{
			this.supplierService = supplierServices;
			this.userContextService = userContextService;
		}

		[HttpPost]
		public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto dto)
		{
			var result = await supplierService.CreateSupplierAsync(dto);
			return Ok(result);
		}

		[HttpGet("{id}")]   ////محتاجه تظبيط 
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.RoleGroup)]
		public async Task<IActionResult> GetSupplierById(int id, [FromQuery] ProductPaginationForSupplierQuery query)
		{
			var result = await supplierService.GetSupplierByIdAsync(id, query);
			return Ok(result);
		}

		[HttpGet("paginated")]
		public async Task<IActionResult> GetSuppliersPaged([FromQuery] SupplierQueryParameters query)
		{
			var route = $"{Request.Scheme}://{Request.Host}{Request.Path}";
			var result = await supplierService.GetPaginatedSuppliersAsync(query);
			return Ok(result);
		}

		[HttpPut]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Supplier},{AppRoles.Admin}")]
		public async Task<IActionResult> UpdateSupplier([FromQuery] string? userId ,[FromBody] UpdateSupplierDto dto)
		{
			if(string.IsNullOrEmpty(userId))
				userId = userContextService.userId;
			else
			{
				if (!userContextService.IsAdmin && userId != userContextService.userId)
					return Forbid();
			}
			var result = await supplierService.UpdateSupplierAsync(userId, dto);
			return Ok(result);
		}
	}
}
