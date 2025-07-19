using Application.Constants;
using Application.DTO_s;
using Application.DTO_s.SupplierDto_s;
using Application.Interfaces;
using Application.Services;
using Domain.Entity;
using InventoryManagement.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
		[AllowAnonymous]
		[SwaggerOperation( Summary = "Register a new supplier account",
	    Description = @"Creates a new supplier account with associated login credentials and company details.
		The following validations are performed before creation:
		- The email must be unique.
		- The username must be unique.
		- The company name must not already exist.

		If all validations pass:
		- A user account is created for the supplier.
		- The supplier is saved with company details.
		- A success response is returned with confirmation.")]
		public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto dto)
		{
			var result = await supplierService.CreateSupplierAsync(dto);
			return StatusCode(result.StatusCode,result);
		}

		[HttpGet("paginated")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.InventoryManager},{AppRoles.Admin}")]
		[SwaggerOperation( Summary = "Get paginated list of suppliers",
	    Description = @"Retrieves a paginated list of suppliers with optional search and sorting options.
		Supports:
		- Searching by supplier name (case-insensitive).
		- Sorting by any supplier field (e.g. CompanyName).
		- Pagination through `PageNumber` and `PageSize`.")]
		public async Task<IActionResult> GetSuppliersPaged([FromQuery] SupplierQueryParameters query)
		{
			var result = await supplierService.GetPaginatedSuppliersAsync(query);
			return Ok(result);
		}

		[HttpPut("update")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Supplier},{AppRoles.Admin}")]
		[SwaggerOperation( Summary = "Update supplier information",
	    Description = @"Allows a supplier to update their own data, or an admin to update any supplier's data.
		- If user is **Supplier**, they can only update their own `CompanyName` and `Address`.
		- If user is **Admin**, they can also update `IsVerified` and `Notes` For Supplier.")]
		[ServiceFilter(typeof(RequireVerifiedSupplierAttribute))]
		public async Task<IActionResult> UpdateSupplie([FromQuery] string? SupplierId, [FromBody] UpdateSupplierDto dto)
		{
			if (userContextService.IsSupplier)
			{
				var result = await supplierService.UpdateSupplierAsync(userContextService.userId, dto);
				return StatusCode(result.StatusCode, result);
			}
			else
			{
				if (string.IsNullOrEmpty(SupplierId))
					return StatusCode(404, "SupplierId is Required");

				var result = await supplierService.UpdateSupplierAsync(SupplierId, dto);
				return StatusCode(result.StatusCode, result);
			}
		}
		[HttpPost("upload-Document")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Supplier)]
		//[TypeFilter(typeof(ValidateImageAttribute),Arguments = new object[] { "ImageFile", true })]
		[SwaggerOperation( Summary = "Upload supplier verification ",
	    Description = @"Allows a supplier to upload a verification document such as a tax card or business registration.
        - The uploaded document will be reviewed by an Admin to verify the supplier's identity.")]
		public async Task<IActionResult> UploadTaxDocument([FromForm] FileUploadDto file)
		{
			var supplierId = userContextService.userId;
			var result = await supplierService.UploadSupplierTaxDocumentAsync(supplierId, file);
			return StatusCode(result.StatusCode, result);
		}


		[HttpPost("verify")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Admin)]
		[SwaggerOperation(
		Summary = "Change supplier verification status",
		Description = @"Allows an admin to change the verification status of a supplier.
		Possible statuses:
		- **Verified**: Marks the supplier as verified.
		- **Rejected**: Marks the supplier as rejected and requires a rejection reason.
		- **Pending**: Resets the status to pending.

		Note:
		- Only users with **Admin** role can perform this operation.
		- If status is Rejected, a `RejectionReason` must be provided." )]
		public async Task<IActionResult> ChangeVerificationStatus([FromForm] ChangeSupplierVerificationStatusDto dto)
		{
			var result = await supplierService.ChangeVerificationStatusAsync(dto);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("verification")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.Supplier}")]
		[SwaggerOperation( Summary = "Get supplier verification status",
     	Description = @"Returns the verification status of a supplier.

		- If the logged-in user is a **supplier**, the system will fetch the status based on their Token value.
		- If the logged-in user is an **admin**, they must provide the `supplierId` in the query to retrieve the status of a specific supplier.

		Returned data includes:
		- **Status**: The current verification status (e.g., Pending, Verified, Rejected).
		- **Reason**: If status is Rejected, shows the rejection reason; otherwise, 'No enter reason'.
		Authorization:
		- Accessible by users with roles: **Admin** or **Supplier**." )]
		public async Task<IActionResult> GetVerificationStatusById([FromQuery] string? supplierId)
		{
			if (userContextService.IsSupplier)
			{
				var result = await supplierService.GetVerificationStatusByIdAsync(userContextService.userId);
				return StatusCode(result.StatusCode, result);
			}
			else
			{
				if (string.IsNullOrEmpty(supplierId))
				{
					return BadRequest("SupplierId is required for non-supplier users");
				}
				var result = await supplierService.GetVerificationStatusByIdAsync(supplierId);
				return StatusCode(result.StatusCode, result);
			}
		}

		[HttpGet("by-verification")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Admin)]
		[SwaggerOperation( Summary = "Get suppliers by verification status",
	    Description = @"Retrieves a list of suppliers based on their verification status.
		- If `isVerified` is **true**, returns only verified suppliers.
		- If `isVerified` is **false**, returns only unverified suppliers.
		- If `isVerified` is **null**, returns all suppliers regardless of their verification status.
		Authorization:
		- Accessible by users with role: **Admin** only." )]
		public async Task<IActionResult> GetByVerificationStatus([FromQuery] bool? isVerified)
		{
			var result = await supplierService.GetSuppliersByVerificationStatusAsync(isVerified);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("simulate-receiving/{purchaseOrderId}")]
		[SwaggerOperation( Summary = "Simulate supplier receiving for a purchase order",
	    Description = @"Simulates the supplier's receiving process for a purchase order with status **Sent** or **PartiallyReceived**.

		- For each item in the order:
		  - There's an 85% chance the full quantity will be marked as received.
		  - 10% chance a partial quantity will be received.

		- The purchase order status will be updated accordingly (**PartiallyReceived** or **Received**).
		Authorization:
		- Requires authentication as **Supplier** " )]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Supplier)]
		[ServiceFilter(typeof(RequireVerifiedSupplierAttribute))]
		public async Task<IActionResult> SupplierReceiving(int purchaseOrderId)
		{
			var result = await supplierService.SimulateSupplierReceivingAsync(purchaseOrderId);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("purchase-orders")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Supplier)]
		[SwaggerOperation( Summary = "Get purchase orders that are pending or partially received ",
	   Description = "Returns all purchase orders for the currently authenticated supplier that" +
			" are either still pending (not fully processed) or partially received.")]
		[ServiceFilter(typeof(RequireVerifiedSupplierAttribute))]
		public async Task<IActionResult> GetSupplierPendingPurchaseOrders()
		{
			var result = await supplierService.GetPendingPurchaseOrdersForSupplierAsync();
			return StatusCode(result.StatusCode, result);
		}

	}
}
