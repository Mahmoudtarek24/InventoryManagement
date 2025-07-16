using Application.DTO_s.StockMovementDto_s;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
		public async Task<IActionResult> RecordSale([FromBody] List<RecordStockMovementDto> dtoList)
		{
			var result = await stockMovementService.RecordSaleAsync(dtoList);
			return Ok(result);
		}

		[HttpPost("transfer")]
		public async Task<IActionResult> RecordTransfer([FromBody] TransferStockDto dto)
		{
			var result = await stockMovementService.RecordTransferAsync(dto);
			return Ok(result);
		}

		[HttpPost("adjustment")]
		public async Task<IActionResult> RecordAdjustment([FromBody] AdjustmentDto dto)
		{
			var result = await stockMovementService.RecordAdjustmentAsync(dto);
			return Ok(result);
		}

		[HttpGet("by-product/{productId}")]
		public async Task<IActionResult> GetMovementsByProduct(int productId, [FromQuery] int? pageNumber)
		{
			int page = pageNumber ?? 1;	
			var result = await stockMovementService.GetMovementsByProductAsync(productId, page);
			return Ok(result);
		}


		[HttpGet("by-warehouse/{warehouseId}")]
		public async Task<IActionResult> GetMovementsByWarehouse(int warehouseId, [FromQuery] int? pageNumber)
		{
			int page = pageNumber ?? 1;
			var result = await stockMovementService.GetMovementsByWarehouseAsync(warehouseId,page);
			return Ok(result);
		}

		[HttpGet("all")]
		public async Task<IActionResult> GetStockMovements([FromQuery] StockMovementQueryParameters query)
		{
			var result = await stockMovementService.GetStockMovementsAsync(query);
			return Ok(result);
		}
	}
}
