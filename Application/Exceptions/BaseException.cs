using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
	public abstract class BaseException :Exception
	{
		public int StatusCode { get; }	
		public string ErrorCode { get; }

		protected BaseException(string message,string errorCode, int statusCode) :base(message) 
		{
			ErrorCode = errorCode;
			StatusCode = statusCode;
		}

		protected BaseException(string message, string errorCode, int statusCode, Exception innerException): base(message,innerException)
		{
			ErrorCode = errorCode;
			StatusCode = statusCode;
		}

	}
}
