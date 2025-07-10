using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Constants
{
	public static class AppRoles //can not used as paramter on method 
	{
		public const string Admin = "Admin";
		public const string InventoryManager = "InventoryManager";
		public const string SalesViewer = "SalesViewer";
		public const string Supplier = "Supplier";
		public const string Bearer = "Bearer";
		public const string RoleGroup = "Admin,Supplier,InventoryManager";
		public const string AllRole = "Admin,Supplier,InventoryManager,SalesViewer";
		public const string SystemRole = "Admin,InventoryManager,SalesViewer";

	}
}
