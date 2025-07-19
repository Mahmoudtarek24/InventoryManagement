using Application.Exceptions;
using Application.ResponseDTO_s;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace InventoryManagement.Middleware
{
	public class GlobalExceptionHandlingMiddleware 
		//this middleware response for catch all error ocure on application and return The appropriate response (json response)
	{
		private readonly RequestDelegate _next ; ////represent next middleware on pipline

		public GlobalExceptionHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
		}
		public async Task InvokeAsync(HttpContext context)//// essential method on middleware ,respons to try to 
														  ////execute code if  get any exception will catch it
		{
			try
			{
				await _next(context);  ///pass request to another middlware
			}
			catch(Exception ex) 
			{
				await HandleExceptionAsync(context, ex); /////this our method will handle excption
			}

		}
		/// will analyzing error her based on it will build our response 
		private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{ 
			var response=context.Response;  ///This represent response will back to user based on her request 
			response.ContentType = "application/json"; /// specify the response content type will return on response header
													   /// this will return json not html 


			var errorResponse = exception switch
			{
				BaseException baseEx =>
					   ApiResponse<string>.ErrorResponse(baseEx.Message, baseEx.ErrorCode, baseEx.StatusCode),

				UnauthorizedAccessException => 
				       ApiResponse<string>.ErrorResponse("You are not authorized to access", "UNAUTHORIZED", 401),
			
				ArgumentException argEx =>
				       ApiResponse<string>.ErrorResponse(argEx.Message, "INVALID_ARGUMENT", 400),

				KeyNotFoundException => 
				       ApiResponse<string>.ErrorResponse("The requested resource does not exist", "NOT_FOUND", 404),

				_ => ApiResponse<string>.ErrorResponse($"An internal server error occurred: {exception.Message}", "INTERNAL_SERVER_ERROR", 500),
			};

			response.StatusCode = errorResponse.StatusCode;

			var jsonResponse = JsonSerializer.Serialize(errorResponse,new JsonSerializerOptions
			{
				PropertyNamingPolicy=JsonNamingPolicy.CamelCase, ///used to conver  ErrorCode =>errorCode  
				WriteIndented = true,	
			}); /////used to convert normal text to json response 
			await response.WriteAsync(jsonResponse);	
		}
	}
}

