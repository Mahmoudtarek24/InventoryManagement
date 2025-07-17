using Application.Constants;
using Application.DTO_s.InventoryDto_s;
using Application.Interfaces;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InventoryController : ControllerBase
	{
		private readonly IInventoryService inventoryService;
		public InventoryController(IInventoryService inventoryService)
		{ 
			this.inventoryService = inventoryService;
		}

		[HttpGet("product/{productId}/warehouse/{warehouseId}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Get inventory by product and warehouse",
	    Description = "Retrieves the inventory information for a specific product in a specific warehouse." )]
		public async Task<IActionResult> GetByProductAndWarehouse(int productId, int warehouseId)
		{
			var result = await inventoryService.GetInventoryByProductAndWarehouseAsync(productId, warehouseId);
			return StatusCode(result.StatusCode,result);
		}


		[HttpGet("product/{productId}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.SystemRole)]
		[SwaggerOperation( Summary = "Get inventory by product",
    	Description = "Retrieves all inventory records for the specified product across all warehouses.")]
		public async Task<IActionResult> GetByProduct(int productId)
		{
			var result = await inventoryService.GetInventoryByProductAsync(productId);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("low-stock")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.SystemRole)]
		[SwaggerOperation( Summary = "Get low stock alerts",
	    Description = "Returns a list of products whose stock quantity is below the specified threshold." +
		" Useful for generating restock alerts ")]
		public async Task<IActionResult> GetLowStockAlerts([FromQuery] int threshold)
		{
			var result = await inventoryService.GetLowStockAlertsAsync(threshold);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("warehouse/{warehouseId}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.SystemRole)]
		[SwaggerOperation( Summary = "Get inventory by warehouse",
	    Description = "Returns a paginated and filtered list of inventory items for a specific warehouse." +
		" Supports search and pagination." )]
		public async Task<IActionResult> GetByWarehouse(int warehouseId, [FromQuery] InventorQueryParameter query)
		{
			var result = await inventoryService.GetInventoryByWarehouseAsync(query, warehouseId);
			return StatusCode(result.StatusCode, result);
		}
	}
}
