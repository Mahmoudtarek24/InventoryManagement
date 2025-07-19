using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.ResponseDTO_s
{
	public class ApiResponse<T>
	{
		public int StatusCode { get; set; }
		public T? Data { get; set; }
		public List<string> ErrorMessages { get; set; }
		public bool IsSuccess { get; set; }
		public string? Message { get; set; }
		public string ErrorCode { get; set; }
		public ApiResponse()
		{
			IsSuccess = true;
			ErrorMessages = new List<string>();
		}
		public static ApiResponse<T> Success(T Data, int StatusCode, string message = null)
		{
			return new ApiResponse<T>
			{
				StatusCode = StatusCode,
				Data = Data,
				Message = message
			};
		}
		public static ApiResponse<T> SuccessWithWarnings(T Data, int StatusCode, List<string> Errors)
		{
			var obj = new ApiResponse<T>
			{
				StatusCode = StatusCode,
				Data = Data
			};

			if (Errors != null && Errors.Any())
			{
				obj.ErrorMessages = Errors;
			}
			return obj;
		}
		public static ApiResponse<T> Failuer(int StatusCode, string error, string errorCode = "NOTFOUND_ERROR")
		{
			return new ApiResponse<T>
			{
				StatusCode = StatusCode,
				IsSuccess = false,
				ErrorCode = errorCode,
				ErrorMessages = new List<string> { error }
			};
		}
		public static ApiResponse<T> ErrorResponse(string message, string errorCode, int statusCode)
		{
			return new ApiResponse<T>
			{
				StatusCode = statusCode,
				IsSuccess = false,
				ErrorCode = errorCode,
				Message = message
			};
		}
		public static ApiResponse<T> Unauthorized(string message = "You are not authorized to access")
		{
			return new ApiResponse<T>
			{
				StatusCode = 401,
				IsSuccess = false,
				ErrorCode = "UNAUTHORIZED",
				Message = message,
			};
		}
		public static ApiResponse<T> ValidationError(string error)
		{
			return new ApiResponse<T>
			{
				StatusCode = 400,
				IsSuccess = false,
				ErrorCode = "VALIDATION_ERROR",
				ErrorMessages = new List<string> { error }
			};
		}
		public static ApiResponse<T> ValidationError(List<string> errors)
		{
			var result = new ApiResponse<T>
			{
				StatusCode = 400,
				IsSuccess = false,
				ErrorCode = "VALIDATION_ERROR",
			};
			result.ErrorMessages.AddRange(errors);
			return result;
		}
		public static ApiResponse<T> BadRequest(string error)
		{
			return new ApiResponse<T>
			{
				StatusCode = 400,
				IsSuccess = false,
				ErrorCode = " BadRequest_ERROR",
				ErrorMessages = new List<string> { error }
			};
		}
	}
}
