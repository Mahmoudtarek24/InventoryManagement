using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventoryManagement.Filters
{
	public class ValidateModelAttribute : ActionFilterAttribute
	{
		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var serviceProvider = context.HttpContext.RequestServices;

			foreach (var argument in context.ActionArguments)
			{
				var argumentType = argument.Value?.GetType();
				if (argumentType == null) continue;

				var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
				var validator = serviceProvider.GetService(validatorType) as IValidator;

				if (validator != null)
				{
					var validationResult = await validator.ValidateAsync(new ValidationContext<object>(argument.Value));
					if (!validationResult.IsValid)
					{
						var errors = validationResult.Errors.Select(e => new {
							Field = e.PropertyName,
							Message = e.ErrorMessage
						}).ToList();
						context.Result = new BadRequestObjectResult(errors);
						return;
					}
				}
			}

			await next();
		}
	}
}


