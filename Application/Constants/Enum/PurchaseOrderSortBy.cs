using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Constants.Enum
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum PurchaseOrderSortBy
	{
		CompanyName,
		TotalCost
	}
}
