using Application.Constants;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
		public async Task<IActionResult> GetByProductAndWarehouse(int productId, int warehouseId)
		{
			var result = await inventoryService.GetInventoryByProductAndWarehouseAsync(productId, warehouseId);
			return Ok(result);
		}


		[HttpGet("product/{productId}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.SystemRole)]
		public async Task<IActionResult> GetByProduct(int productId)
		{
			var result = await inventoryService.GetInventoryByProductAsync(productId);
			return Ok(result);
		}

		[HttpGet("low-stock")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.SystemRole)]
		public async Task<IActionResult> GetLowStockAlerts([FromQuery] int threshold)
		{
			var result = await inventoryService.GetLowStockAlertsAsync(threshold);
			return Ok(result);
		}
	}
}
