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
			var productDto = supplier.Products.Select(e => e.ToResponseDto()).ToList();
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
			existingSupplier.CompanyName = dto.CompanyName;
			existingSupplier.Address = dto.Address;
			existingSupplier.LastUpdateOn = DateTime.Now;

			if (userContextService.IsAdmin)
			{
				existingSupplier.IsVerified = dto.IsVerified ?? existingSupplier.IsVerified;
				existingSupplier.Notes = dto.Notes ?? existingSupplier.Notes;
			}
		}

	}
}
///i was implement this class to be respond on mapping and let service only handle bussines logic (speration of Concerns)
/// service handle only bussines logic , make code reusable can used on more places  , more Readability  ,
