using Domain.Entity;
using Domain.Parameters;
using Domain.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
	public interface ISupplierRepository : IGenaricRepository<Supplier>
	{
		Task<bool> IsCompanyNameExistsAsync(string companyName);
		Task<(int,List<SupplierInfo>)> SupplierWithProductCountAsync(BaseFilter filter);
	}
}

