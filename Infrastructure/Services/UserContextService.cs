using Application.Constants;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
	public class UserContextService : IUserContextService
	{
		public string[] Roles { get ;private set; }
		public string userId { get; private set; }
		public string Route {  get; private set; }

		public void SetUser(string[] Roles, string userId,string route)
		{
			this.Roles = Roles;
			this.userId = userId;	
			this.Route =  route ?? string.Empty; ;	
		}
		public bool IsInRole(string roleName)
		{
			return Roles?.Contains(roleName) ?? false;
		}

		public bool IsAdmin => IsInRole(AppRoles.Admin);
		public bool IsInventoryManager => IsInRole(AppRoles.InventoryManager);
		public bool IsSalesViewer => IsInRole(AppRoles.SalesViewer);
		public bool IsSupplier => IsInRole(AppRoles.Supplier);
	}
}

//this code implement Encapsulation by make properites readonly and can not exchange her value outside this class only
//used SetUser method inside middleware to set her value 
///data=> filds,properites ,we can see the data is ( filds,properites ) expres on class like (userid ,proice)
///Encapsulation the way we encapsulate data of class and prevent to access it outside class only throw way we define it 
///like methos to set data or propreites  
///inside method or prodrites i was but my own condition to check from passing value 
///when i was implement encapsulation her iwas prevent any another services or class to set value to my "data"

///encapsulation mean encapsulate data +controll how access it 
