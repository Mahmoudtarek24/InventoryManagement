using Application.Constants.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO_s.PurchaseOrder
{
	public class PurchaseOrderQueryParameter : BaseQueryParameters 
	{ 
		public PurchaseOrderSortBy SortOptions { get; set; }
		public PurchaseStatus PurchaseOrderStatus { get; set; }
	}
}
