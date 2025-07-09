using Application.Constants;
using Application.DTO_s;
using Application.DTO_s.ProductDto_s;
using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly IProductServices productServices;
		public ProductController(IProductServices productServices)
		{
			this.productServices = productServices;
		}

		[HttpPost]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.Supplier)]
		public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
		{
			var result = await productServices.CreateProductAsync(dto);
			return Ok(result);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetProductById(int id)
		{
			var result = await productServices.GetProductByIdAsync(id);
			return Ok(result);
		}

		[HttpGet("by-category/{categoryId}")]
		public async Task<IActionResult> GetProductsByCategory(int categoryId)
		{
			var result = await productServices.GetProductsByCategoryAsync(categoryId);
			return Ok(result);
		}

		[HttpGet("by-supplier/{supplierId}")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = AppRoles.RoleGroup)]
		public async Task<IActionResult> GetProductsBySupplier(int supplierId, [FromQuery] SupplierProductsQueryParameters qP)
		{
			//////we should remove query her only page id
			var result = await productServices.GetProductsBySupplierAsync(supplierId, qP);
			return Ok(result);
		}

		[HttpPut("{productId}/availability")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		public async Task<IActionResult> ChangeAvailability(int productId, [FromQuery] bool status)
		{
			var result = await productServices.ChangeAvailabilityAsync(productId, status);
			return Ok(result);
		}

		[HttpGet("list")]
		[Authorize(AuthenticationSchemes = AppRoles.Bearer, Roles = $"{AppRoles.Admin},{AppRoles.InventoryManager}")]
		public async Task<IActionResult> GetProductsPaged([FromQuery] BaseQueryParameters query)
		{
			var result = await productServices.GetProductsWithPaginationAsync(query);
			return Ok(result);
		}

		[HttpPost("bulk")]
		public async Task<IActionResult> BulkCreateProducts([FromBody] List<CreateProductDto> dtos)
		{
			var result = await productServices.BulkCreateProductsAsync(dtos);
			return Ok(result);
		}
	}
}
