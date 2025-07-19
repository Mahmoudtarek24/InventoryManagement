using Application.Constants;
using Application.DTO_s.PurchaseOrder;
using Application.DTO_s.PurchaseOrderDto_s;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Create a new purchase order",
	    Description = "Creates a new purchase order for a specific supplier. " +
			"Validates supplier status and product list before creation. ")]
		public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDto dto)
		{
			var result = await purchaseOrderService.CreatePurchaseOrderAsync(dto);
			return StatusCode(result.StatusCode,result);
		}


		[HttpGet("{purchaseId}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.RoleGroup)]
		[SwaggerOperation( Summary = "Get Purchase Order by ID",
	    Description = "Returns details of a specific purchase order, including items and supplier info. " +
				  "Suppliers can only access their own sent or received orders." )]
		public async Task<IActionResult> GetById(int purchaseId)
		{
			var result = await purchaseOrderService.GetPurchaseOrderByIdAsync(purchaseId);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("all")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Admin)]
		[SwaggerOperation( Summary = "Get all purchase orders with filters and pagination",
	    Description = "Retrieves a paginated list of purchase orders filtered by status, search term, " +
		"and sorted by different criteria. Admin access only." )]
		public async Task<IActionResult> GetAll([FromQuery] PurchaseOrderQueryParameter query)
		{
			var result = await purchaseOrderService.GetPurchaseOrdersWithPaginationAsync(query);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("by-supplier/{supplierId}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Get purchase orders for a specific supplier",
	    Description = "Returns all purchase orders that belong to the given supplier by supplier ID" +
			".\n\nAuthorized Roles: Admin, InventoryManager" )]
		public async Task<IActionResult> GetBySupplier(int supplierId)
		{
			var result = await purchaseOrderService.GetOrdersBySupplierAsync(supplierId);
			return Ok(result);
		}
		
        [HttpPut("{id}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Update an  purchase order",
	    Description = "Updates fields such as status, warehouse, or expected delivery date of a purchase order." +
			"\n\nAuthorized Roles: Admin, InventoryManager\n\nAllowed Status for update: Draft, Cancelled" )]
		public async Task<IActionResult> UpdatePurchaseOrder(int id, [FromBody] UpdatePurchaseOrderDto dto)
		{
			var result = await purchaseOrderService.UpdatePurchaseOrderAsync(id, dto);
			return StatusCode(result.StatusCode, result);
		}
	
		[HttpPost("{id}/items")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Add item(s) to an existing purchase order",
     	Description = "Allows authorized users (Admin, InventoryManager) to add new items or increase quantity for existing items in a purchase order.\n\n" +
				  "• Single item or list of items can be submitted.\n" +
				  "• If item already exists, quantity will be increased instead of duplicated.\n" )]
		public async Task<IActionResult> AddOrderItem(int id, [FromBody] AddOrderItemDto dto)
		{
			var result = await purchaseOrderService.AddOrderItemAsync(id, dto);
			return StatusCode(result.StatusCode, result);
		}
		
		[HttpDelete("{id}/items")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Remove items from a purchase order",
    	Description = @"This endpoint removes specific products from an existing purchase order by product IDs.
		- You cannot remove all items; at least one item must remain in the order. " )]
		public async Task<IActionResult> RemoveOrderItem(int id, [FromBody] RemoveOrderItemsDto dto )
		{
			var result = await purchaseOrderService.RemoveOrderItemsAsync(id, dto);
			return StatusCode(result.StatusCode, result);
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
