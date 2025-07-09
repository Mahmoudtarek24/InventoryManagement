using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
	public class NotFoundException :BaseException
	{
		public NotFoundException(string entityName ,object id)
			        :base($"Entity {entityName} with {id} was not found.", "NOT_FOUND", 404)
		{
				
		}
	}
}
