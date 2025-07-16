using Application.Constants;
using Application.DTO_s;
using Application.DTO_s.SupplierDto_s;
using Application.Interfaces;
using Application.Services;
using Domain.Entity;
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

		[HttpGet("{id}")]   ////محتاجه تظبيط 
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.RoleGroup)]
		public async Task<IActionResult> GetSupplierById(int id, [FromQuery] ProductPaginationForSupplierQuery query)
		{
			var result = await supplierService.GetSupplierByIdAsync(id, query);
			return Ok(result);
		}

		[HttpGet("paginated")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.InventoryManager},{AppRoles.Admin}")]
		public async Task<IActionResult> GetSuppliersPaged([FromQuery] SupplierQueryParameters query)
		{
			var result = await supplierService.GetPaginatedSuppliersAsync(query);
			return Ok(result);
		}

		[HttpPut]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Supplier},{AppRoles.Admin}")]
		public async Task<IActionResult> UpdateSupplier([FromQuery] string? userId ,[FromBody] UpdateSupplierDto dto)
		{
			if(string.IsNullOrEmpty(userId))
				userId = userContextService.userId;
			else
			{
				if (!userContextService.IsAdmin && userId != userContextService.userId)
					return Forbid();
			}
			var result = await supplierService.UpdateSupplierAsync(userId, dto);
			return Ok(result);
		}


		[HttpPost("upload-Document")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Supplier)]
		public async Task<IActionResult> UploadTaxDocument([FromForm] FileUploadDto file)
		{
			var supplierId = userContextService.userId;
			var result = await supplierService.UploadSupplierTaxDocumentAsync(supplierId, file);
			return Ok(result);
		}

		[HttpPost("verify")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Admin)]
		public async Task<IActionResult> ChangeVerificationStatus([FromForm] ChangeSupplierVerificationStatusDto dto)
		{
			var result = await supplierService.ChangeVerificationStatusAsync(dto);
			return Ok(result);
		}

		[HttpGet("verification")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.Supplier}")]
		public async Task<IActionResult> GetVerificationStatusById([FromQuery] string? supplierId)
		{
			if (userContextService.IsSupplier)
			{
				var result = await supplierService.GetVerificationStatusByIdAsync(userContextService.userId, userContextService.IsSupplier);
				return Ok(result);
			}
			else
			{
				if (string.IsNullOrEmpty(supplierId))
				{
					return BadRequest("SupplierId is required for non-supplier users");
				}
				var result = await supplierService.GetVerificationStatusByIdAsync(supplierId,userContextService.IsSupplier);
				return Ok(result);
			}
		}

		[HttpGet("by-verification")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Admin)]
		public async Task<IActionResult> GetByVerificationStatus([FromQuery] bool? isVerified)
		{
			var result = await supplierService.GetSuppliersByVerificationStatusAsync(isVerified);
			return Ok(result);
		}

		[HttpPost("simulate-receiving/{purchaseOrderId}")]
		public async Task<IActionResult> SupplierReceiving(int purchaseOrderId)
		{
			var result = await supplierService.SimulateSupplierReceivingAsync(purchaseOrderId);
			return Ok(result);
		}
	}
}
