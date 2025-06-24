using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s
{
	public class ConfirmationResponseDto
	{
		public string Message { get; set; }
		public ConfirmationStatus status { get; set; }	
	}
	public enum ConfirmationStatus
	{
		SoftDeleted,
		HardDeleted,
		Created,
		Failed,
		Activated,
		Deactivated,
		Updated,
		upload
	}
}
