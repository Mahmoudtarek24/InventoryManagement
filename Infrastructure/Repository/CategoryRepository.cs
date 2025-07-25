﻿using Domain.Entity;
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

		public async Task<(List<Category>, int)> GetCategorysWithFiltersAsync(BaseFilter catF)
		{
			var query = context.Categories.Where(e => !e.IsDeleted).AsQueryable();
			int totalCount = await query.CountAsync();
			query = query.Skip((catF.PageNumber - 1) * catF.PageSize).Take(catF.PageSize);
			var result = await query.ToListAsync();
			return (result, totalCount);
		}

		public async Task<List<Category>> GetAllActiveCategoryAsync() =>
						await context.Categories.AsNoTracking().Where(c => !c.IsDeleted)
									 .ToListAsync();

		public async Task<bool> IsValidCategoryIdAsync(int categoryId) =>
			          await  context.Categories.AnyAsync(e=>e.CategoryId== categoryId&&!e.IsDeleted);

		public async Task<List<int>> GetValidCategoryIdsAsync(List<int> categoryIds) =>
			 await context.Categories.Where(c => categoryIds.Contains(c.CategoryId)).Select(c => c.CategoryId).ToListAsync();
	}
}

