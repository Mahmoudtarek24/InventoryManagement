using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Constants.Enum
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum MovementType
	{
		ReceivedFromSupplier = 1,
		ReturnedToSupplier = 2,
		ManualAdjustmentIncrease = 3,
		ManualAdjustmentDecrease = 4,
		TransferIn = 5,
		TransferOut = 6,
		Sale = 7,
		SaleReturn = 8,
		InventoryCorrection = 9
	}
}
