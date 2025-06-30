using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
	public enum PurchaseOrderStatus
	{
		/// <summary> "The order has been created but has not been submitted yet </summary>
		Draft = 0,
		/// <summary> The order has been sent to the supplier (awaiting supplier response or execution) </summary>
		Sent = 1,
		/// <summary> Only part of the products has been received </summary>
		PartiallyReceived = 3,
		/// <summary> All requested products have been received </summary>
		Received = 4,
		/// <summary> The order has been cancelled by the warehouse </summary>
		Cancelled = 5
	}
}
