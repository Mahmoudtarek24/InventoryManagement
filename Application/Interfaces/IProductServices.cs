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

namespace Application.Interfaces
{
	public interface IProductServices
	{
		Task<ApiResponse<ProductResponseDto>> CreateProductAsync(CreateProductDto dto);
		Task<ApiResponse<List<ProductResponseDto>>> BulkCreateProductsAsync(List<CreateProductDto> dtos);
		Task<ApiResponse<ProductResponseDto>> GetProductByIdAsync(int id);
		Task<ApiResponse<ConfirmationResponseDto>> SoftDeleteProductAsync(int id);
		Task<PagedResponse<List<ProductResponseDto>>> GetProductsWithPaginationAsync(ProductQueryParameters productQuery, string route);
		Task<ApiResponse<List<ProductWithCategoryRespondDto>>> GetProductsByCategoryAsync(int categoryId);
		Task<ApiResponse<ConfirmationResponseDto>> ChangeAvailabilityAsync(int productId, bool status);
		Task<PagedResponse<List<ProductsBySupplierResponseDto>>> GetProductsBySupplierAsync(int supplierId, SupplierProductsQueryParameters qP);

		//Task<ApiResponse<ProductResponseDto>> UpdateProductAsync(int id, UpdateProductDto dto); //dint change name if have quentity on stick
		//Task<ApiResponse<List<ProductStockAlertDto>>> GetDeletedProductsWithStockAsync();
		//Task<ApiResponse<List<ProductStockAlertDto>>> GetLowStockProductsAsync(int threshold);
		//Task<ApiResponse<List<ProductWithWarehouseDto>>> GetProductsByWarehouseAsync(int warehouseId);
		//Task<ApiResponse<ConfirmationResponseDto>> UpdateProductStockAsync(int id, int newStock);


	}
}
