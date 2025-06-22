using Application.Constants;
using Application.DTO_s.SupplierDto_s;
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
		public SupplierResponseDto MapSupplierToResponseDto(Supplier supplier, string[] Roles, ProductPaginationForSupplierQuery qP)
		{
			var rolesSet=Roles.ToHashSet();
			var responseDto = supplier.ToBasicResponseDto();
			var productDto = supplier.Products.Select(e => e.ToResponseDto()).ToList();
			responseDto.Products =
				PagedResponse<IEnumerable<ProductBaseRespondDto>>.SimpleResponse(productDto, qP.PageNumber, qP.PageSize, qP.TotalCount);

			if (rolesSet.Contains(AppRoles.Admin) || rolesSet.Contains(AppRoles.Supplier) || rolesSet.Contains(AppRoles.InventoryManager))
			{
				responseDto.TaxDocumentPath = supplier.TaxDocumentPath;
				responseDto.Address = supplier.Address;
				responseDto.LastUpdateOn = supplier.LastUpdateOn;
				responseDto.Notes = supplier.Notes;
			}

			if (rolesSet.Contains(AppRoles.Admin) || rolesSet.Contains(AppRoles.Supplier))
			{
				responseDto.UserId = supplier.UserId;
			}
			return responseDto;
		}

		public SupplierListRespondDto MapSuppliedrToResponseDtooo(SupplierInfo supplier, string Role)
		{
			var responseDto = new SupplierListRespondDto();

			responseDto.Address = supplier.Address;
			responseDto.LastUpdateOn = supplier.LastUpdateOn;
			responseDto.SupplierId = supplier.SupplierId;
			responseDto.CompanyName = supplier.CompanyName;
			responseDto.CreateOn = supplier.CreateOn;
			responseDto.IsVerified = supplier.IsVerified;
			responseDto.PhoneNumber = supplier.PhoneNumber;

			if (Role==AppRoles.Admin)
			{
				responseDto.Email = supplier.Email;
			    responseDto.UserId=supplier.userId;
			}
			return responseDto;
		}
	}
}
///i was implement this class to be respond on mapping and let service only handle bussines logic (speration of Concerns)
/// service handle only bussines logic , make code reusable can used on more places  , more Readability  ,
