using InventoryManagement.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace InventoryManagement.Filters
{
	public class ValidateImageAttribute : ActionFilterAttribute
	{
		private readonly ImageSettings imageSettings ;
		private readonly bool requiredToApply;
		private readonly string FormFieldName;
		//public ValidateImageAttribute(string formFieldName, IOptions<ImageSettings> options, bool requiredToApply)
		//{
		//	imageSettings = options.Value;	
		//	this.requiredToApply = requiredToApply;
		//	this.FormFieldName = formFieldName;
		//}

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var errors = new string[1]; 
			int errorIndex = 0;

			var file = context.HttpContext.Request.Form.Files[FormFieldName];

			if (file == null)
			{
				if (requiredToApply)
				{
					context.Result = new BadRequestObjectResult("Image is required");
					return;
				}
				else
				{
					await next();
					return;
				}
			}

			if (file.Length > imageSettings.MaxFileSize)
				errors[errorIndex++] = $"File size exceeds maximum allowed size ({imageSettings.MaxFileSize} bytes)";

			var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
			if (string.IsNullOrEmpty(extension) || !imageSettings.AllowedExtensions.Contains(extension))
				errors[errorIndex++] = $"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", imageSettings.AllowedExtensions)}";

			if (errors.Any())
				context.Result = new BadRequestObjectResult(new { errors });
			else
				await next();
		}

	}
}
