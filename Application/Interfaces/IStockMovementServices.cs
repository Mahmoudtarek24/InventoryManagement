using Application.DTO_s.StockMovementDto_s;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.StockMovementResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface IStockMovementServices
	{
		Task RecordPurchaseAsync(List<RecordsPurchase> dto);
		Task<ApiResponse<ConfirmationResponseDto>> RecordSaleAsync(RecordStockMovementDto dto);
		Task<ApiResponse<ConfirmationResponseDto>> RecordTransferAsync(TransferStockDto dto);
		Task<ApiResponse<ConfirmationResponseDto>> RecordAdjustmentAsync(AdjustmentDto dto);
		Task<ApiResponse<List<StockMovementResponseDto>>> GetMovementsByProductAsync(int productId, int pageNumber);
		Task<ApiResponse<List<StockMovementResponseDto>>> GetMovementsByWarehouseAsync(int warehouseId, int pageNumber);
		Task<PagedResponse<List<StockMovementResponseDto>>> GetStockMovementsAsync(StockMovementQueryParameters query);
	}
}
