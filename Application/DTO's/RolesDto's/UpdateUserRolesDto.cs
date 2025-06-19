using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.RolesDto_s
{
	public class UpdateUserRolesDto
	{
		public string userId { get; set; }
		public List<string> roles { get; set; }	
	}
}
