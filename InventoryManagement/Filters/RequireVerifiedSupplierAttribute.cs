using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryManagement.Filters
{
	public class RequireVerifiedSupplierAttribute : Attribute, IAsyncAuthorizationFilter
	{
		private readonly IUserContextService contextService;
		public RequireVerifiedSupplierAttribute(IUserContextService contextService)
		{
			this.contextService = contextService;
		}
		public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			if (contextService.IsSupplier)
			{
				var supplier = context.HttpContext.User;

				var isVerified = supplier.FindFirst("IsSupplierVerified")?.Value;

				if (string.IsNullOrEmpty(isVerified) || isVerified == "False")
					context.Result = new UnauthorizedObjectResult(
					               "You are not verified. Please upload your documents and wait for admin approval.");
			}
		}
	}
}