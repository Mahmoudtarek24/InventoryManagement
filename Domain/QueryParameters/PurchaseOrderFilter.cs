using Domain.Enum;
using Domain.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.QueryParameters
{
	public class PurchaseOrderFilter :BaseFilter
	{
		public PurchaseOrderStatus? Status { get; set; }
	}
}
