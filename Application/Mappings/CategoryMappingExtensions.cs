using Application.ResponseDTO_s.CategoryResponse;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
	public static class CategoryMappingExtensions
	{
		public static CategoryResponseDto ToResponseDto(this Category category)
		{
			return new CategoryResponseDto()
			       {
				CategoryId = category.CategoryId,
				CreateOn = category.CreateOn,
				Description = category.Description,
				DisplayOrder = category.DisplayOrder,
				IsDeleted = category.IsDeleted,
				LastUpdateOn = category.LastUpdateOn,
				Name = category.Name,
			       };
		}
	}
}
