using Domain.Entity;
using Domain.Interface;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
	public class ProductRepository : GenaricRepository<Category>, IProductRepository
	{
		public ProductRepository(InventoryManagementDbContext context) : base(context) { }
		
		public async Task<bool> HasProductsForCategoryAsync(int categoryId) =>
		    	await context.Products.AnyAsync(e => e.CategoryId == categoryId);
	}
}
