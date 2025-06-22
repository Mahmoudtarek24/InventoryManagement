using Application.ResponseDTO_s.CategoryResponse;
using Application.ResponseDTO_s.ProductResponse;
using Application.ResponseDTO_s.SupplierResponse;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
	public static class MappingExtensions
	{
		public static CategoryResponseDto ToResponseDto(this Category category)
		{
			if (category == null)
				return null;

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
		public static ProductResponseDto ToResponseDto(this Product product)
		{
			if (product == null)
				return null;

			return new ProductResponseDto()
			{
				CategoryId= product.CategoryId,	
				CreateOn= product.CreateOn,
				Barcode = product.Barcode,
				IsAvailable = product.IsAvailable,	
				IsDeleted= product.IsDeleted,
				LastUpdateOn= product.LastUpdateOn,
				Name= product.Name,
				Price = product.Price,
				ProductId= product.ProductId,
			};
		}
		public static ProductWithCategoryRespondDto ToResponseDtoWithCategory(this Product product)
		{
			if (product == null)
				return null;

			return new ProductWithCategoryRespondDto()
			{
				ProductId = product.ProductId,
				Name = product.Name,
				Barcode = product.Barcode,
				Price = product.Price,
				IsAvailable = product.IsAvailable,
				IsDeleted = product.IsDeleted,
				CreateOn = product.CreateOn,
				LastUpdateOn = product.LastUpdateOn,
				CategoryId = product.CategoryId,
				CategoryName = product.Category.Name
			};
		}
		public static SupplierResponseDto ToBasicResponseDto(this Supplier supplier)
		{
			if (supplier == null)
				return null;

			return new SupplierResponseDto()
			{
				IsDeleted = supplier.IsDeleted,
				CompanyName = supplier.CompanyName,
				CreateOn = supplier.CreateOn,
				IsVerified = supplier.IsVerified,
				SupplierId = supplier.SupplierId,
			};
		}

	}
}
