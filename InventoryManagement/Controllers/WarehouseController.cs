using Application.Constants;
using Application.DTO_s;
using Application.DTO_s.WarehouseDto_s;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WarehouseController : ControllerBase
	{
		private readonly IWarehouseService warehouseService;
		public WarehouseController(IWarehouseService warehouseService)
		{
			this.warehouseService = warehouseService;	
		}

		[HttpPost]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Admin)]
		public async Task<IActionResult> Create([FromForm] CreateWarehouseDto dto)
		{
			var result = await warehouseService.CreateWarehouseAsync(dto);
			return Ok(result);
		}

		[HttpGet("{id}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		public async Task<IActionResult> GetById(int id)
		{
			var result = await warehouseService.GetWarehouseByIdAsync(id);
			return Ok(result);
		}

		
		[HttpGet]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Admin)]
		public async Task<IActionResult> GetAll([FromQuery] BaseQueryParameters query)
		{
			var result = await warehouseService.GetWarehousesAsync(query);
			return Ok(result);
		}
	}
}
