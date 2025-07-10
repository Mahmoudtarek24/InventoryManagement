using Application.DTO_s.SupplierDto_s;
using Application.ResponseDTO_s.SupplierResponse;
using Application.ResponseDTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;
using Microsoft.AspNetCore.Http;
using Application.ResponseDTO_s.PurchaseOrder;
using Application.DTO_s;

namespace Application.Interfaces
{
	public interface ISupplierServices
	{
		Task<ApiResponse<ConfirmationResponseDto>> CreateSupplierAsync(CreateSupplierDto dto);
		Task<ApiResponse<SupplierResponseDto>> GetSupplierByIdAsync(int id, ProductPaginationForSupplierQuery queryParamter);
		Task<PagedResponse<List<SupplierListRespondDto>>> GetPaginatedSuppliersAsync(SupplierQueryParameters qP);
		Task<ApiResponse<ConfirmationResponseDto>> UpdateSupplierAsync(string id, UpdateSupplierDto dto);
		//Task<ApiResponse<ConfirmationResponseDto>> SoftDeleteCategoryAsync(int id);

		// verification 
		Task<ApiResponse<ConfirmationResponseDto>> ChangeVerificationStatusAsync(ChangeSupplierVerificationStatusDto dto);
		Task<ApiResponse<SupplierVerificationStatusBaseRespondDto>> GetVerificationStatusByIdAsync(string supplierId, bool IsSupplier);
		Task<ApiResponse<List<SupplierVerificationStatusRespondDto>>> GetSuppliersByVerificationStatusAsync(bool? isVerified=null);

		//TaxDocument
		Task<ApiResponse<ConfirmationResponseDto>> UploadSupplierTaxDocumentAsync(string supplierId,FileUploadDto file);

		///Task<ApiResponse<supplier>> reated-this-mont(int id);
		//Task<ApiResponse<int>> CountSuppliersAsync();
		//Task<ApiResponse<int>> CountVerifiedSuppliersAsync();
		//Task<ApiResponse<int>> CountSuppliersCreatedThisMonthAsync();

		/// can change price 

		Task<ApiResponse<PurchaseOrderDetailsResponseDto>> SimulateSupplierReceivingAsync(int purchaseOrderId);

	}
}
