﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
namespace Application.Constants.Enum
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum EgyptianGovernorate
	{
		Cairo,
		Alexandria,
		PortSaid,
		Dakahlia,
	}
}
