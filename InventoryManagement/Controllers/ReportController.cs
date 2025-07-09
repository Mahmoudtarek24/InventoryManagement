using Application.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReportController : ControllerBase
	{
		private readonly IProductServices productServices;
		public ReportController(IProductServices productServices)
		{
			this.productServices = productServices;
		}

		[HttpGet("product/{productId}/purchase-history")]
		public async Task<IActionResult> GetProductPurchaseHistory(int productId)
		{
			var result = await productServices.GetProductPurchaseHistoryAsync(productId);
			return Ok(result);
		}
	}
}
