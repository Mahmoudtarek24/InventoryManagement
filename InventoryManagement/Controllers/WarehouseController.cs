using Application.Constants;
using Application.DTO_s;
using Application.DTO_s.WarehouseDto_s;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
		[SwaggerOperation( Summary = "Create a new warehouse",
	    Description = "Creates a new warehouse and generates a unique serial number for it. Only accessible by Admins." )]
		public async Task<IActionResult> Create([FromForm] CreateWarehouseDto dto)
		{
			var result = await warehouseService.CreateWarehouseAsync(dto);
			return StatusCode(result.StatusCode,result);
		}

		[HttpGet("{id}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Get warehouse by ID",
	     Description = "Retrieves the warehouse details using its unique ID. Accessible by Admins and Inventory Managers." )]
		public async Task<IActionResult> GetById(int id)
		{
			var result = await warehouseService.GetWarehouseByIdAsync(id);
			return StatusCode(result.StatusCode, result);
		}

		
		[HttpGet]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Admin)]
		[SwaggerOperation( Summary = "Get all warehouses with pagination",
	    Description = "Retrieves a paginated list of all warehouses. Only accessible by Admins." )]
		public async Task<IActionResult> GetAll([FromQuery] PaginationQueryParameters query)
		{
			var result = await warehouseService.GetWarehousesAsync(query);
			return StatusCode(result.StatusCode, result);
		}
	}
}
