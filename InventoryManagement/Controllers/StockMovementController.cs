using Application.Constants;
using Application.DTO_s.StockMovementDto_s;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StockMovementController : ControllerBase
	{
		private readonly IStockMovementServices stockMovementService;
		public StockMovementController(IStockMovementServices stockMovementServices)
		{
			this.stockMovementService = stockMovementServices;	
		}

		[HttpPost("sale")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Record bulk product sales",
	    Description = "Validates stock and records sales across warehouses." +
			" Only accessible by Admin or InventoryManager roles.")]
		public async Task<IActionResult> RecordSale([FromBody] RecordStockMovementDto dtoList)
		{
			var result = await stockMovementService.RecordSaleAsync(dtoList);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("transfer")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Transfer stock between warehouses",
	    Description = "Transfers quantities of products from a source warehouse to a destination warehouse. " +
			"Only accessible by Admin or InventoryManager roles.")]
		public async Task<IActionResult> RecordTransfer([FromBody] TransferStockDto dto)
		{
			var result = await stockMovementService.RecordTransferAsync(dto);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("adjustment")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Manually adjust inventory levels",
    	Description = "Register manual inventory adjustments (increase or decrease) for a specific warehouse. " +
				  "Used for lost, damaged, or found stock. Only accessible by Admin or InventoryManager roles." )]
		public async Task<IActionResult> RecordAdjustment([FromBody] AdjustmentDto dto)
		{
			var result = await stockMovementService.RecordAdjustmentAsync(dto);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("by-product/{productId}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Get all stock movements for a specific product",
     	Description = "Retrieves all stock movement transactions (in/out) related to a specific product, paginated by page number. Accessible to Admin and InventoryManager roles only.")]
		public async Task<IActionResult> GetMovementsByProduct(int productId, [FromQuery] int? pageNumber)
		{
			int page = pageNumber ?? 1;	
			var result = await stockMovementService.GetMovementsByProductAsync(productId, page);
			return StatusCode(result.StatusCode, result);
		}


		[HttpGet("by-warehouse/{warehouseId}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Get all stock movements for a specific warehouse ",
	Description = "Retrieves all stock movement transactions (in/out) related to a specific warehouse, Accessible to Admin and InventoryManager roles only.")]
		public async Task<IActionResult> GetMovementsByWarehouse(int warehouseId, [FromQuery] int? pageNumber)
		{
			int page = pageNumber ?? 1;
			var result = await stockMovementService.GetMovementsByWarehouseAsync(warehouseId,page);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("all")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Get all stock movements with filters and pagination",
		Description = @"Retrieves all stock movement transactions with optional filters :
	   Only accessible by Admin and InventoryManager roles." )]
		public async Task<IActionResult> GetStockMovements([FromQuery] StockMovementQueryParameters query)
		{
			var result = await stockMovementService.GetStockMovementsAsync(query);
			return StatusCode(result.StatusCode, result);
		}
	}
}
