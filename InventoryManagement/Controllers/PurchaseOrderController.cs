using Application.Constants;
using Application.DTO_s.PurchaseOrder;
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
		public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
		{
			this.purchaseOrderService = purchaseOrderService;
		}

		[HttpPost]
		//[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDto dto)
		{
			var result = await purchaseOrderService.CreatePurchaseOrderAsync(dto);
			return Ok(result);
		}

	}
}
