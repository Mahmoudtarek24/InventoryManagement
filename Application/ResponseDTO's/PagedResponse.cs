using Application.ResponseDTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ResponseDTO_s
{
	public class PagedResponse<T> :ApiResponse<T>
	{
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public Uri FirstPage { get; set; }
		public Uri LastPage { get; set; }
		public int TotalPages { get; set; }
		public int TotalRecords { get; set; }
		public Uri NextPage { get; set; }
		public Uri PreviousPage { get; set; }
		
		public PagedResponse()
		{
			IsSuccess = true;
			ErrorMessages = null;
		}

		public static PagedResponse<T> SimpleResponse(T data, int pageNumber, int pageSize, int totalRecords,string message=null)
		{
			return new PagedResponse<T>()
			{
				Message=message,	
				PageNumber = pageNumber,
				PageSize = pageSize,
				Data = data,
				TotalPages = Convert.ToInt32(Math.Ceiling(((double)totalRecords / (double)pageSize)))
			};
		}

		//public static PagedResponse<T> CompleteResponse(T data, int pageNumber, int pageSize)
		//{
		//	return new PagedResponse<T>()
		//	{
		//		PageNumber = pageNumber,
		//		PageSize = pageSize,
		//		Data = data,
		//	};
		//}
	}
}

