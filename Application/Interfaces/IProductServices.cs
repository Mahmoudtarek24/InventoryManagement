using Application.DTO_s.ProductDto_s;
using Application.ResponseDTO_s.ProductResponse;
using Application.ResponseDTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ResponseDTO_s.CategoryResponse;
using Application.DTO_s.CategoryDto_s;
using Application.ResponseDTO_s.PurchaseOrder;
using Application.DTO_s;

namespace Application.Interfaces
{
	public interface IProductServices
	{
		Task<ApiResponse<ProductResponseDto>> CreateProductAsync(CreateProductDto dto);
		Task<ApiResponse<List<ProductResponseDto>>> BulkCreateProductsAsync(List<CreateProductDto> dtos);
		Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(int id);
		Task<ApiResponse<ConfirmationResponseDto>> SoftDeleteProductAsync(int id);
		Task<PagedResponse<List<ProductResponseDto>>> GetProductsWithPaginationAsync(BaseQueryParameters productQuery);
		Task<ApiResponse<List<ProductWithCategoryRespondDto>>> GetProductsByCategoryAsync(int categoryId);
		Task<ApiResponse<ConfirmationResponseDto>> ChangeAvailabilityAsync(int productId, bool status);
		Task<ApiResponse<ProductsBySupplierResponseDto>> GetProductsBySupplierAsync(string supplierId, PaginationQueryParameters qP);

		 Task<ApiResponse<List<PurchaseHistoryProductResponseDto>>> GetProductPurchaseHistoryAsync(int productId); //on report controller 
		Task<ApiResponse<ConfirmationResponseDto>> BulkUpdateProductPricesAsync(List<UpdateProductPriceDto> dtos);
	}
}
