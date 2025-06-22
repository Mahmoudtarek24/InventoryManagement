using Application.DTO_s.CategoryDto_s;
using Application.DTO_s;
using Application.Interfaces;
using Application.ResponseDTO_s.CategoryResponse;
using Application.ResponseDTO_s;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO_s.SupplierDto_s;
using Application.ResponseDTO_s.SupplierResponse;
using Domain.Interface;
using Domain.Entity;
using Application.Constants;
using Application.ResponseDTO_s.ProductResponse;
using Application.Mappings;
using Domain.Parameters;
using Application.Constants.Enum;

namespace Application.Services
{
	public class SupplierServices : ISupplierServices
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IAuthService authService;
		private readonly RoleBasedSupplierMapper mapper;
		public SupplierServices(IUnitOfWork unitOfWork, IAuthService authService, RoleBasedSupplierMapper mapper)
		{
			this.unitOfWork = unitOfWork;
			this.authService = authService;
			this.mapper = mapper;
		}
		public async Task<ApiResponse<ConfirmationResponseDto>> CreateSupplierAsync(CreateSupplierDto dto)
		{
			// Check if email already exists
			bool emailExists = await authService.IsEmailUniqueAsync(dto.Email);
			if (emailExists)
				throw new Exception();
			// throw new ConflictException($"Supplier with email '{dto.Email}' already exists.");

			bool companyNameExists = await unitOfWork.SupplierRepository.IsCompanyNameExistsAsync(dto.CompanyName);
			if (companyNameExists)
				throw new Exception();
			// throw new ConflictException($"Supplier with company name '{dto.CompanyName}' already exists.");

			// Check if username already exists
			bool userNameExists = await authService.IsUserNameUniqueAsync(dto.UserName);
			if (userNameExists)
				throw new Exception();
			// throw new ConflictException($"Username '{dto.UserName}' already exists.");

			var userId = await authService.CreateSupplierAsync(dto);
			var supplier = new Supplier
			{
				CompanyName = dto.CompanyName,
				Address = dto.Address,
				CreateOn = DateTime.Now,
				UserId = userId,
			};

			await unitOfWork.BeginTransactionAsync();
			await unitOfWork.SupplierRepository.AddAsync(supplier);
			await unitOfWork.CommitTransaction();

			ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			{
				Message = $"Supplier '{supplier.CompanyName}' with ID {supplier.SupplierId} Created Successfully",
				status = ConfirmationStatus.Created
			};
			return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 201);
		}

		public async Task<ApiResponse<SupplierResponseDto>> GetSupplierByIdAsync(int supplierId, string[] Roles, ProductPaginationForSupplierQuery qP)
		{
			var supplier = await unitOfWork.SupplierRepository.GetByIdAsync(supplierId);
			if (supplier is null)
				throw new Exception();
			///

			var productFilter = new BaseFilter
			{
				PageSize = qP.PageSize,
				PageNumber = qP.PageNumber,
			};
			var (productCount, productwithpage) = await unitOfWork.ProductRepository.GetProductsBySupplierAsync(supplierId, productFilter);
			supplier.Products = productwithpage;
			qP.TotalCount= productCount;	

			var respondDto = mapper.MapSupplierToResponseDto(supplier, Roles, qP);

			return ApiResponse<SupplierResponseDto>.Success(respondDto, 200);
		}

		public async Task<PagedResponse<List<SupplierListRespondDto>> GetAllSuppliersAsync(string Roles, SupplierQueryParameters qP)
		{
			var Filter = new BaseFilter
			{
				PageNumber = qP.PageSize,
				PageSize = qP.PageSize,
				searchTearm = qP.searchTearm,
				SortAscending = qP.SortAscending,
				SortBy = qP.SortBy.ToString(),
			};
			var (tottalCount, suppliers) = await unitOfWork.SupplierRepository.SupplierWithProductCountAsync(Filter);

			qP.TottalCount = tottalCount;

			List<SupplierListRespondDto> supp = new List<SupplierListRespondDto>();
			foreach (var supplier in suppliers)
			{
				supp.Add(mapper.MapSuppliedrToResponseDtooo(supplier, Roles));
			}

			var smallmapp=PagedResponse<List<SupplierListRespondDto>>.SimpleResponse(supp,qP.PageSize,qP.PageSize,qP.TottalCount);	
			//return smallmapp.AddPagingInfo(tottalCount)
			return smallmapp;
		}

	}
}

     