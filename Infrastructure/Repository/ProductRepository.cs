using Domain.Entity;
using Domain.Interface;
using Domain.Parameters;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
	public class ProductRepository : GenaricRepository<Product>, IProductRepository
	{
		public ProductRepository(InventoryManagementDbContext context) : base(context) { }


		public async Task<bool> HasProductsForCategoryAsync(int categoryId) =>
				await context.Products.AsNoTracking().AnyAsync(e => e.CategoryId == categoryId);

		public async Task<bool> IsDuplicateProductNameInCategoryAsync(string productName, int categoryId) =>
			 await context.Products.AsNoTracking().AnyAsync(e => e.Name == productName && e.CategoryId == categoryId);

		public async Task<bool> IsBarcodeUniqueAsync(string barcode) =>
			await context.Products.AnyAsync(e => e.Barcode == barcode);

		public async Task<Product?> GetIfExistsAndNotDeletedAsync(int id) =>
				 await context.Products.Where(e => e.ProductId == id)
						.Select(c => new Product
						{
							ProductId = c.ProductId,
							IsDeleted = c.IsDeleted
						}).FirstOrDefaultAsync();


		public async Task<(List<Product>, int)> GetProductsWithFiltersAsync(BaseFilter prodF)
		{
			var query = context.Products.Where(e => !e.IsDeleted).AsQueryable();


			if (!string.IsNullOrEmpty(prodF.searchTearm))
				query = query.Where(e => 
				      e.Name.Contains(prodF.searchTearm, StringComparison.OrdinalIgnoreCase) ||
					  e.Barcode.Contains(prodF.searchTearm, StringComparison.OrdinalIgnoreCase));

			int totalCount = await query.CountAsync();

			if (!string.IsNullOrEmpty(prodF.SortBy))
			{
				switch (prodF.SortBy.ToLower())
				{
					case "name":
						query = prodF.SortAscending ? query.OrderBy(e => e.Name) : query.OrderByDescending(e => e.Name);
						break;

					case "price":
						query = prodF.SortAscending ? query.OrderBy(e => e.Price) : query.OrderByDescending(e => e.Price);
						break;
					case "isavailable":
						query = prodF.SortAscending ? query.OrderBy(e => e.IsAvailable) : query.OrderByDescending(e => e.IsAvailable);
						break;

					default:
						query = prodF.SortAscending ? query.OrderBy(e => e.CreateOn) : query.OrderByDescending(e => e.CreateOn);
						break;
				}
			}

			query = query.Skip((prodF.PageNumber - 1) * prodF.PageSize).Take(prodF.PageSize);
			var result = await query.ToListAsync();
			return (result, totalCount);
		}

		public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
		{
			return await context.Products.Include(e=>e.Category)
				       .Where(e=>e.CategoryId==categoryId && !e.IsDeleted).ToListAsync();
		}

		public async Task<(int,List<Product>)> GetProductsBySupplierAsync(int supplierId, BaseFilter prodF)
		{
			var query=context.Products.Where(e=>e.SupplierId==supplierId)
				                                   .AsNoTracking().AsQueryable();	

			var productsCount = await query.CountAsync();	
			if(productsCount < 20)
				return (productsCount, query.ToList());


			return (productsCount, query.Skip((prodF.PageNumber-1)* prodF.PageSize).Take(prodF.PageSize).ToList());	
		}
	}
}

