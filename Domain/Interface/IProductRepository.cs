using Domain.Entity;
using Domain.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
	public interface IProductRepository :IGenaricRepository<Product>
	{
		Task<bool> HasProductsForCategoryAsync(int categoryId);
		Task<bool> IsDuplicateProductNameInCategoryAsync(string name, int categoryId);
		Task<bool> IsBarcodeUniqueAsync(string barcode);
		Task<Product?> GetIfExistsAndNotDeletedAsync(int id);
		Task<(List<Product>, int)> GetProductsWithFiltersAsync(BaseFilter prodF);
		Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
	}
}
