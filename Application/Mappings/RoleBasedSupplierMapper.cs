using Application.Constants;
using Application.DTO_s.SupplierDto_s;
using Application.Interfaces;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.ProductResponse;
using Application.ResponseDTO_s.SupplierResponse;
using Domain.Entity;
using Domain.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
	public class RoleBasedSupplierMapper
	{
		private readonly IUserContextService userContextService;
		public RoleBasedSupplierMapper(IUserContextService userContextService)
		{
			this.userContextService = userContextService;
		}
		public SupplierResponseDto MapSupplierToResponseDto(Supplier supplier, ProductPaginationForSupplierQuery qP)
		{
			var responseDto = supplier.ToBasicResponseDto();
			var productDto = supplier.Products.Select(e => e.ToResponseDto(userContextService)).ToList();
			responseDto.Products =
				PagedResponse<IEnumerable<ProductBaseRespondDto>>.SimpleResponse(productDto, qP.PageNumber, qP.PageSize, qP.TotalCount);

			if (userContextService.IsAdmin || userContextService.IsSupplier || userContextService.IsInventoryManager)
			{
				responseDto.TaxDocumentPath = supplier.TaxDocumentPath;
				responseDto.Address = supplier.Address;
				responseDto.LastUpdateOn = supplier.LastUpdateOn;
				responseDto.Notes = supplier.Notes;
			}

			if (userContextService.IsAdmin || userContextService.IsSupplier)
			{
				responseDto.UserId = supplier.UserId;
			}
			return responseDto;
		}

		public SupplierListRespondDto MapToSupplierListDto(SupplierInfo supplier)
		{
			var responseDto = new SupplierListRespondDto();

			responseDto.Address = supplier.Address;
			responseDto.LastUpdateOn = supplier.LastUpdateOn;
			responseDto.SupplierId = supplier.SupplierId;
			responseDto.CompanyName = supplier.CompanyName;
			responseDto.CreateOn = supplier.CreateOn;
			responseDto.IsVerified = supplier.IsVerified;
			responseDto.PhoneNumber = supplier.PhoneNumber;

			if (userContextService.IsAdmin)
			{
				responseDto.Email = supplier.Email;
				responseDto.UserId = supplier.userId;
			}
			return responseDto;
		}

		public void MapUpdateDtoToSupplier(UpdateSupplierDto dto, Supplier existingSupplier)
		{
			if (userContextService.IsSupplier)
			{
				existingSupplier.CompanyName = dto.CompanyName;
				existingSupplier.Address = dto.Address;
			}
			if (userContextService.IsAdmin)
			{
				existingSupplier.IsVerified = dto.IsVerified ?? existingSupplier.IsVerified;
				existingSupplier.Notes = dto.Notes ?? existingSupplier.Notes;
			}
			existingSupplier.LastUpdateOn = DateTime.Now;

		}
		public ProductsBySupplierResponseDto MapSupplierToResponseDto(Supplier supplier) 
		{
			var responseDto = new ProductsBySupplierResponseDto();

			responseDto.SupplierId = supplier.SupplierId;
			responseDto.CompanyName = supplier.CompanyName;
			responseDto.Address = supplier.Address;
			responseDto.IsVerified = supplier.IsVerified;
			responseDto.VerificationStatus =(Constants.Enum.VerificationStats)supplier.VerificationStatus;

			return responseDto;
		}
		public ProductBaseRespondDto MapProductToResponseDto(Product product)
		{
			var responseDto = new ProductBaseRespondDto();

			// Basic product information always available
			responseDto.ProductId= product.ProductId;
			responseDto.Name = product.Name;
			responseDto.Price = product.Price;
			responseDto.CategoryId = product.CategoryId;
			responseDto.CreateOn = product.CreateOn;
		
			// Role-based additional information
			if (userContextService.IsAdmin || userContextService.IsInventoryManager)
			{
				responseDto.Barcode = product.Barcode;
				responseDto.IsAvailable = product.IsAvailable;
				responseDto.IsDeleted = product.IsDeleted;
				responseDto.LastUpdateOn = product.LastUpdateOn;
			}

			return responseDto;
		}
	}
}
///i was implement this class to be respond on mapping and let service only handle bussines logic (speration of Concerns)
/// service handle only bussines logic , make code reusable can used on more places  , more Readability  ,
