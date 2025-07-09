using Application.Constants.Enum;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s
{
	public class ConfirmationResponseDto
	{
		public string Message { get; set; }
		public ConfirmationStatus status { get; set; }	
	}
	
}
