using Domain.Entity;
using Domain.Interface;
using Domain.Parameters;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
	public class CategoryRepository : GenaricRepository<Category>, ICategoryRepository
	{
		public CategoryRepository(InventoryManagementDbContext context) : base(context) { }

		public async Task<bool> IsCategoryNameUniqueAsync(string name) =>
			await context.Categories.AsNoTracking()
				.AnyAsync(p => p.Name.ToLower() == name.ToLower());

		public async Task<bool> IsDisplayOrderTakenAsync(int displayOrder) =>
			await context.Categories
				.AsNoTracking()
				.AnyAsync(c => c.DisplayOrder == displayOrder);

		public async Task<Category?> GetIfExistsAndNotDeletedAsync(int id) =>
			 await context.Categories.Where(e => e.CategoryId == id)
				.Select(c => new Category
				{
					CategoryId = c.CategoryId,
					IsDeleted = c.IsDeleted
				}).FirstOrDefaultAsync();

		public async Task<(List<Category>,int)> GetCategorysWithFiltersAsync(CategoryFilter catF)
		{
			var query = context.Categories.Where(e => !e.IsDeleted).AsQueryable();

			if (!string.IsNullOrEmpty(catF.searchTearm))
				query = query.Where(e => e.Name.Contains(catF.searchTearm, StringComparison.OrdinalIgnoreCase));

			int totalCount = await context.Categories.Where(e => !e.IsDeleted).CountAsync();
			if (!string.IsNullOrEmpty(catF.SortBy))
			{
				switch (catF.SortBy.ToLower())
				{
					case "Name":
						query = catF.SortAscending ? query.OrderBy(e => e.Name) : query.OrderByDescending(e => e.Name);
						break;

					case "DisplayOrder":
						query = catF.SortAscending ? query.OrderBy(e => e.DisplayOrder) : query.OrderByDescending(e => e.DisplayOrder);
						break;

					default:
						query = catF.SortAscending ? query.OrderBy(e => e.CreateOn) : query.OrderByDescending(e => e.CreateOn);
						break;
				}
			}

			query=query.Skip((catF.PageNumber-1)*1).Take(catF.PageSize);	
			var result=await query.ToListAsync();
			return (result, totalCount);
		}
	}
}
