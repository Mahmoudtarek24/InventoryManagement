using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
	public class ForbiddenException : BaseException
	{
		public ForbiddenException(string message = "You do not have permission to access this resource.")
		: base(message, "FORBIDDEN", 403)
		{
		}
	}
}
