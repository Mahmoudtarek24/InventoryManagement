using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Constants.Enum
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum PurchaseOrderStatus
	{
		Draft = 0,
		Sent = 1,
		PartiallyReceived = 3,
		Received = 4,
		Cancelled = 5
	}
}
