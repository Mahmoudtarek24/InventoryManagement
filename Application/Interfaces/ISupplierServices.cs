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

namespace Application.Interfaces
{
	public interface ISupplierServices
	{
		Task<ApiResponse<ConfirmationResponseDto>> CreateSupplierAsync(CreateSupplierDto dto);
		Task<ApiResponse<SupplierResponseDto>> GetSupplierByIdAsync(int id, ProductPaginationForSupplierQuery queryParamter);
		Task<PagedResponse<List<SupplierListRespondDto>>> GetPaginatedSuppliersAsync(SupplierQueryParameters qP);
		Task<ApiResponse<ConfirmationResponseDto>> UpdateSupplierAsync(int id, UpdateSupplierDto dto);
		//Task<ApiResponse<ConfirmationResponseDto>> SoftDeleteCategoryAsync(int id);

		// verification 
		Task<ApiResponse<ConfirmationResponseDto>> ChangeVerificationStatusAsync(ChangeSupplierVerificationStatusDto dto);
		Task<ApiResponse<SupplierVerificationStatusBaseRespondDto>> GetVerificationStatusByIdAsync(int supplierId);
		Task<ApiResponse<List<SupplierVerificationStatusRespondDto>>> GetSuppliersByVerificationStatusAsync(bool? isVerified=null);

		//TaxDocument
		Task<ApiResponse<ConfirmationResponseDto>> UploadSupplierTaxDocumentAsync(int id,IFormFile file);

		///Task<ApiResponse<supplier>> reated-this-mont(int id,IFormFile file);
		//Task<ApiResponse<int>> CountSuppliersAsync();
		//Task<ApiResponse<int>> CountVerifiedSuppliersAsync();
		//Task<ApiResponse<int>> CountSuppliersCreatedThisMonthAsync();

		//////have end point for supplier to detict the order status


		////////Very important , we should have like method to get product from supplier ask supplier to enter her product 
		///to register on system
		
		/// can change price 

	}
}
