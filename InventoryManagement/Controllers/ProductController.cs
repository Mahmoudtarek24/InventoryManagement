using Application.Constants;
using Application.DTO_s;
using Application.DTO_s.ProductDto_s;
using Application.Interfaces;
using Application.Services;
using InventoryManagement.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace InventoryManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[ServiceFilter(typeof(RequireVerifiedSupplierAttribute))]
	public class ProductController : ControllerBase
	{
		private readonly IProductServices productServices;
		private readonly IUserContextService userContextService;
		public ProductController(IProductServices productServices,IUserContextService userContextService)
		{
			this.productServices = productServices;
            this.userContextService = userContextService;
		}

		[HttpPost]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Supplier)]
		[SwaggerOperation(
		Summary = "Create a new product (Supplier only)",
		Description = @"Allows an authorized verification supplier to create a new product. ")]
		public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
		{
			var result = await productServices.CreateProductAsync(dto);
			return StatusCode(result.StatusCode,result);
		}

		[HttpGet("{id}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.AllRole)]
		[SwaggerOperation(
		Summary = "Get product by ID",
		Description = @"Retrieves a product by its unique ID.
		Returns 404 if the product is not found.")]
		public async Task<IActionResult> GetProductById(int id)
		{
			var result = await productServices.GetProductByIdAsync(id);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("by-category/{categoryId}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.SystemRole)]
		[SwaggerOperation(
		Summary = "Get products by category ID",
		Description = @"Retrieves a list of products that belong to a specific category.
		The category must exist and must not be soft-deleted.
		If the category exists but has no products, the response will be successful with an empty list.")]
		public async Task<IActionResult> GetProductsByCategory(int categoryId)
		{
			var result = await productServices.GetProductsByCategoryAsync(categoryId);
			return StatusCode(result.StatusCode, result);
		}


		[HttpGet("by-supplier")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.AllRole)]
		[SwaggerOperation(
	    Summary = "Get products for a specific supplier",
	    Description = @"Returns a paginated list of products for a specific supplier. 
        - If the current user is a supplier, their own products will be returned automatically.
        - If the user is an admin or has other roles, the supplierId must be provided as a query parameter.
        - The response includes supplier details and their products.
        - The products are returned with pagination metadata." )]
		public async Task<IActionResult> GetProductsBySupplier([FromQuery] string? supplierId, [FromQuery] PaginationQueryParameters qP)
		{
			if (userContextService.IsSupplier)
			{
				var result = await productServices
						 .GetProductsBySupplierAsync(userContextService.userId,  qP);
				return StatusCode(result.StatusCode, result);
			}
			else
			{
				if (string.IsNullOrEmpty(supplierId))
					return BadRequest("SupplierId is required for non-supplier users");
				
				var result = await productServices.GetProductsBySupplierAsync(supplierId, qP);
				return StatusCode(result.StatusCode, result);
			}
		}

		[HttpPut("{productId}/availability")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		[SwaggerOperation( Summary = "Change product availability status",
	    Description = @"Allows an Admin or Inventory Manager to update the availability status of a product.
		- If the product is already in the desired status, a message is returned.
		- Otherwise, the status is updated (IsAvailable = true/false). " )]
		public async Task<IActionResult> ChangeAvailability(int productId, [FromQuery] bool status)
		{
			var result = await productServices.ChangeAvailabilityAsync(productId, status);
			return Ok(result);
		}

		[HttpGet("list")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.RoleGroup)]
		[SwaggerOperation(
	    Summary = "Get paginated list of products with filtering and sorting",
		Description = @"Returns a paginated list of products.
		You can search by name using the 'searchTearm' query parameter.
		Sorting is supported via 'SortBy' (isavailable, name, price) and 'SortAscending'.
		This endpoint is accessible to authorized users" )]
		public async Task<IActionResult> GetProductsPaged([FromQuery] BaseQueryParameters query)
		{
			var result = await productServices.GetProductsWithPaginationAsync(query);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("bulk")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Supplier)]
		[SwaggerOperation( Summary = "Bulk create multiple products (Supplier only)",
		Description = @"Allows an authorized supplier to create multiple products in a single request.
		- If all entries are invalid, a validation error is returned.
		- If some entries are invalid, valid ones will be processed, and warnings will be included in the response.
		- If all entries are valid, products will be created successfully.")]
		public async Task<IActionResult> BulkCreateProducts([FromBody] List<CreateProductDto> dtos)
		{
			var result = await productServices.BulkCreateProductsAsync(dtos);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPut("bulk-update-prices")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Supplier)]
		[SwaggerOperation( Summary = "Bulk update prices for multiple products (Supplier only)",
	    Description = @"Allows an authorized supplier to update prices of multiple products in one request.
		Each product must:
		- Belong to the current supplier.
		- Have a new price greater than zero.
		- Have a new price different from the existing price.
		Partial success is allowed: valid updates will be processed, and warnings will be returned for invalid items.")]
		public async Task<IActionResult> BulkUpdateProductPrices([FromBody] List<UpdateProductPriceDto> dtos)
		{
			var result = await productServices.BulkUpdateProductPricesAsync(dtos);
			return StatusCode(result.StatusCode, result);
		}
	}
}
