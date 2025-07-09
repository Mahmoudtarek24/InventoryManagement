using Application.DTO_s.StockMovementDto_s;
using Application.ResponseDTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface IStockMovementServices
	{
		//Task<ApiResponse<ConfirmationResponseDto>> RecordPurchaseAsync(RecordStockMovementDto dto);
		Task<ApiResponse<ConfirmationResponseDto>> RecordSaleAsync(RecordStockMovementDto dto);
	//	Task<ApiResponse<ConfirmationResponseDto>> RecordTransferAsync(TransferStockDto dto);
		//Task<ApiResponse<ConfirmationResponseDto>> RecordAdjustmentAsync(AdjustmentDto dto);

		//Task<PagedResponse<List<StockMovementListDto>>> GetStockMovementsAsync(StockMovementQueryParameters query);
		//Task<ApiResponse<List<StockMovementListDto>>> GetMovementsByProductAsync(int productId);
		//Task<ApiResponse<List<StockMovementListDto>>> GetMovementsByWarehouseAsync(int warehouseId);
	}
}
