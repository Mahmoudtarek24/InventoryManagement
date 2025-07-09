using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
	public class ValidationException :BaseException
	{
		public Dictionary<string, string[]> Errors {  get; }
		public ValidationException(Dictionary<string, string[]> errors) 
			        : base("One or more validation errors occurred", "VALIDATION_ERROR", 400)
		{
				
		}
		public ValidationException(string field, string error)
			   : this (new Dictionary<string, string[]> { { field,new[] { error} } })
		{

		}
	}
}
