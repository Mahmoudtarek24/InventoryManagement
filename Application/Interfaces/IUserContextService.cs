using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface IUserContextService   /////need to get this value from middleware when her recieve request
	{
		string[] Roles { get; }
		string userId { get; }	
		bool IsAdmin { get; }	
		bool IsInventoryManager { get;}
		bool IsSalesViewer { get; }
		bool IsSupplier { get; }
	}
}
