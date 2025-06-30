using Application.DTO_s;
using Application.ResponseDTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO_s.PurchaseOrder;
using Application.ResponseDTO_s.PurchaseOrder;
using Application.DTO_s.CategoryDto_s;
using Application.ResponseDTO_s.CategoryResponse;

namespace Application.Interfaces
{
	public interface IPurchaseOrderService
	{
		Task<ApiResponse<ConfirmationResponseDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto);
		Task<ApiResponse<ConfirmationResponseDto>> UpdateDraftPurchaseOrderAsync(UpdatePurchaseOrderDto dto);
		Task<ApiResponse<PurchaseOrderDetailsResponseDto>> GetPurchaseorderByIdAsync(int purchaseId);
		Task<PagedResponse<List<PurchaseOrderListItemResponseDto>>> GetPurchaseOrdersWithPaginationAsync(PurchaseOrderQueryParameter orderQP, string route);
		Task<PagedResponse<List<PurchaseOrderListItemResponseDto>>> GetAllDraftPurchaseOrdersAsync(BaseQueryParameters queryParameters);
	}
}
