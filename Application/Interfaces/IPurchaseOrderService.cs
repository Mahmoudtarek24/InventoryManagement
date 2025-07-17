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
using System.Web.Http;
using Application.DTO_s.PurchaseOrderDto_s;

namespace Application.Interfaces
{
	public interface IPurchaseOrderService
	{
		Task<ApiResponse<ConfirmationResponseDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto);
		Task<ApiResponse<ConfirmationResponseDto>> UpdatePurchaseOrderAsync(int id, [FromBody] UpdatePurchaseOrderDto dto);
		Task<ApiResponse<PurchaseOrderDetailsResponseDto>> GetPurchaseOrderByIdAsync(int purchaseId);
		Task<PagedResponse<List<PurchaseOrderListItemResponseDto>>> GetPurchaseOrdersWithPaginationAsync(PurchaseOrderQueryParameter orderQP);
		Task<ApiResponse<List<PurchaseOrderBySupplierResponseDto>>> GetOrdersBySupplierAsync(int supplierId);
		///will put it on supplier controller amd will get supplierId from token will not ask for it 
		/// also but on Purchase Order controller and will ask for supplier id
		Task<ApiResponse<ConfirmationResponseDto>> AddOrderItemAsync(int purchaseOrderId, AddOrderItemDto dto);
		Task<ApiResponse<ConfirmationResponseDto>> RemoveOrderItemsAsync(int purchaseOrderId, RemoveOrderItemsDto dto);
	}
}
