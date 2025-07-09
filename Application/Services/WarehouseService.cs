using Application.Constants.Enum;
using Application.DTO_s;
using Application.DTO_s.WarehouseDto_s;
using Application.Interfaces;
using Application.Mappings;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.WarehouseResponse;
using Domain.Entity;
using Domain.Interface;
using Domain.Parameters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
	public class WarehouseService : IWarehouseService
	{
		private readonly IUnitOfWork unitOfWork;
		public WarehouseService(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}
		public async Task<ApiResponse<ConfirmationResponseDto>> CreateCategoryAsync(CreateWarehouseDto dto)
		{
			var serialNumber = await GenerateSerialNumberAsync(dto);
			var warehouse = new Warehouse()
			{
				SerialNumber = serialNumber
			};
			await unitOfWork.BeginTransactionAsync();
			await unitOfWork.WarehouseRepository.AddAsync(warehouse);
			await unitOfWork.CommitTransaction();

			var response = new ConfirmationResponseDto()
			{
				status = ConfirmationStatus.Created,
				Message = $"Warehouse created with serial number {warehouse.SerialNumber} and Id {warehouse.WarehouseId}"
			};

			return ApiResponse<ConfirmationResponseDto>.Success(response, 201);
		}


		public async Task<string> GenerateSerialNumberAsync(CreateWarehouseDto dto)
		{
			//// WH-Cairo-000
			var lastSerialNumber = await unitOfWork.WarehouseRepository
										.GetLastSerialNumberByGovernorateAsync(dto.EgyptianGovernorate.ToString());

			if(lastSerialNumber is null)
				return $"WH-{dto.EgyptianGovernorate}-Main";
			
			
			if(int.TryParse(lastSerialNumber.Split('-')[2],out int lastNumber))
				return $"WH-{dto.EgyptianGovernorate}-{(lastNumber + 1):D3}";

			return $"WH-{dto.EgyptianGovernorate}-002";
		}

		public async Task<ApiResponse<WarehouseResponseDto>> GetWarehouseByIdAsync(int id)
		{
			var warehouse=await unitOfWork.WarehouseRepository.GetByIdAsync(id);
			if (warehouse is null)
				throw new Exception();

			var response = new WarehouseResponseDto()
			{
				SerialNumber = warehouse.SerialNumber,
				WarehouseId = warehouse.WarehouseId,
			};
			return ApiResponse<WarehouseResponseDto>.Success(response, 200);
		}

		public async Task<PagedResponse<List<WarehouseResponseDto>>> GetWarehousesAsync(BaseQueryParameters query,string route)
		{
			var parameter = new BaseFilter()
			{
				PageNumber = query.PageNumber,
				PageSize = query.PageSize,
				searchTearm = query.searchTearm,
			};

			var (warehouses, totalCount) = await unitOfWork.WarehouseRepository.GetWarehousesWithFiltersAsync(parameter);

			if (warehouses == null || !warehouses.Any())
				return PagedResponse<List<WarehouseResponseDto>>.SimpleResponse(new List<WarehouseResponseDto>(), 0, 0, 0);

			var warehouseDtos = warehouses.Select(e => new WarehouseResponseDto()
			{
				SerialNumber = e.SerialNumber,
				WarehouseId = e.WarehouseId,
			}).ToList();

			return  PagedResponse<List<WarehouseResponseDto>>
				  .SimpleResponse(warehouseDtos, parameter.PageNumber, parameter.PageSize, totalCount);
		}
	}
}
