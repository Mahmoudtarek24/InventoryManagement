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

		public ApiResponse()
		{
			IsSuccess = true;
			ErrorMessages = new List<string>();
		}
		public static ApiResponse<T> Success(T Data, int StatusCode, string message=null)
		{
			return new ApiResponse<T>
			{
				StatusCode = StatusCode,
				Data = Data,
				Message = message	
			};
		}
		public static ApiResponse<T> SuccessWithWarnings(T Data, int StatusCode,List<string> Errors)
		{
			var obj= new ApiResponse<T>
			{
				StatusCode = StatusCode,
				Data = Data
			};

			if(Errors != null && Errors.Any())
			{
				obj.ErrorMessages = Errors;	
			}	
			return obj;	
		}
		public static ApiResponse<T> Failuer(int StatusCode, string error)
		{
			return new ApiResponse<T>
			{
				StatusCode = StatusCode,
				IsSuccess = false,
				ErrorMessages = new List<string> { error }
			};
		}
		public static ApiResponse<T> Failuer(int StatusCode, List<string> error)
		{
			return new ApiResponse<T>
			{
				StatusCode = StatusCode,
				IsSuccess = false,
				ErrorMessages =  error 
			};
		}
	}
}
