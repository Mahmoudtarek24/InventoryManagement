using Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace InventoryManagement.Middleware
{
	public class UserContextMiddleware
	{
		private readonly RequestDelegate next;
		private readonly ILogger<UserContextMiddleware> logger;
		public UserContextMiddleware(RequestDelegate next, ILogger<UserContextMiddleware> logger)
		{
			this.next = next;
			this.logger = logger;
		}
		public async Task InvokeAsync(HttpContext context, IUserContextService userContext)
		{
			if (context.User.Identity?.IsAuthenticated == true) ///will execute afer authentication to can get data 
			{
				var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var roles  = context.User.FindAll(ClaimTypes.Role).Select(e=>e.Value).ToArray();
				var route = context.Request.Path;

				userContext.SetUser(roles, userId, route);
			}
			await next(context);
		}
	}
}
