using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
	public class UnauthorizedException :BaseException
	{
		public UnauthorizedException(string message = "You are not authorized to access")
		              : base(message, "UNAUTHORIZED", 401)
		{

		}
	}
}
