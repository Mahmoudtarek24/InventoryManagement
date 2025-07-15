using Application.Constants;
using Application.DTO_s.PurchaseOrder;
using Application.DTO_s.PurchaseOrderDto_s;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PurchaseOrderController : ControllerBase
	{
		private readonly IPurchaseOrderService purchaseOrderService;
		private readonly IUserContextService userContextService;
		public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, IUserContextService userContextService)
		{
			this.purchaseOrderService = purchaseOrderService;
			this.userContextService = userContextService;
		}

		[HttpPost]
		//[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDto dto)
		{
			var result = await purchaseOrderService.CreatePurchaseOrderAsync(dto);
			return Ok(result);
		}

		[HttpGet("{purchaseId}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.RoleGroup)]
		public async Task<IActionResult> GetById(int purchaseId)
		{
			var result = await purchaseOrderService.GetPurchaseorderByIdAsync(purchaseId);
			return Ok(result);
		}

		[HttpGet("all")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Admin)]
		public async Task<IActionResult> GetAll([FromQuery] PurchaseOrderQueryParameter query)
		{
			var result = await purchaseOrderService.GetPurchaseOrdersWithPaginationAsync(query);
			return Ok(result);
		}

		[HttpGet("by-supplier/{supplierId}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		public async Task<IActionResult> GetBySupplier(int supplierId)
		{
			var result = await purchaseOrderService.GetOrdersBySupplierAsync(supplierId);
			return Ok(result);
		}
		
        [HttpPut("{id}")]
		public async Task<IActionResult> UpdatePurchaseOrder(int id, [FromBody] UpdatePurchaseOrderDto dto)
		{
			var result = await purchaseOrderService.UpdatePurchaseOrderAsync(id, dto);
			return Ok(result);
		}
	
		[HttpPost("{id}/items")]
		public async Task<IActionResult> AddOrderItem(int id, [FromBody] AddOrderItemDto dto)
		{
			var result = await purchaseOrderService.AddOrderItemAsync(id, dto);
			return Ok(result);
		}
		
		[HttpDelete("{id}/items")]
		public async Task<IActionResult> RemoveOrderItem(int id, [FromBody] RemoveOrderItemsDto dto )
		{
			var result = await purchaseOrderService.RemoveOrderItemsAsync(id, dto);
			return Ok(result);
		}

		//// PUT /purchaseorders/{id}/items/{itemId}
		//[HttpPut("{id}/items/{itemId}")]
		//public async Task<IActionResult> UpdateOrderItem(int id, int itemId, [FromBody] UpdateOrderItemDto dto)
		//{
		//	var result = await purchaseOrderService.UpdateOrderItemAsync(id, itemId, dto);
		//	return Ok(result);
		//}

		//// DELETE /purchaseorders/{id}/items/{itemId}


	}
}
