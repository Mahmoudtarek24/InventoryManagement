﻿using Domain.Entity;
using Domain.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
	public interface ICategoryRepository : IGenaricRepository<Category>
	{
		Task<bool> IsCategoryNameUniqueAsync(string name);
		Task<bool> IsDisplayOrderTakenAsync(int displayOrder);
		Task<Category?> GetIfExistsAndNotDeletedAsync(int id);
		Task<(List<Category>, int)> GetCategorysWithFiltersAsync(BaseFilter catF);
		Task<List<Category>> GetAllActiveCategoryAsync();
		Task<bool> IsValidCategoryIdAsync(int categoryId);
		Task<List<int>> GetValidCategoryIdsAsync(List<int> categoryIds);
	}
}
