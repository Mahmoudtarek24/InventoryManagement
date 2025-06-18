using Application.Interfaces;
using Application.ResponseDTO_s;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
	public static class PaginationHelper
	{

		public static PagedResponse<List<T>> AddPagingInfo<T>(this PagedResponse<List<T>> response , int totalRecords, IUriService uriService, string route)
		{
			var totalPages = ((double)totalRecords / (double)response.PageSize);
			int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

			response.NextPage = response.PageNumber>=1&& response.PageNumber < roundedTotalPages
				? uriService.GetPagnationUri(response.PageNumber + 1, response.PageSize, route) : null;


			response.PreviousPage =
			   response.PageNumber - 1 >= 1 && response.PageNumber <= roundedTotalPages
			   ? uriService.GetPagnationUri(response.PageNumber - 1, response.PageSize, route)
			   : null;

			response.FirstPage = uriService.GetPagnationUri(1, response.PageSize, route);
			response.LastPage = uriService.GetPagnationUri(roundedTotalPages, response.PageSize, route);
			response.TotalRecords = totalRecords;

			return response;
		}
	}
}
