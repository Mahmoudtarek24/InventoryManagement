using Application.Constants.Enum;
using Application.DTO_s;
using Application.DTO_s.StockMovementDto_s;
using Application.DTO_s.WarehouseDto_s;
using Application.Interfaces;
using Application.Mappings;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.CategoryResponse;
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
		private readonly IStockMovementServices stockMovementServices;
		public WarehouseService(IUnitOfWork unitOfWork, IStockMovementServices stockMovementServices)
		{
			this.unitOfWork = unitOfWork;
			this.stockMovementServices = stockMovementServices;
		}
		
		public async Task<ApiResponse<ConfirmationResponseDto>> CreateWarehouseAsync(CreateWarehouseDto dto)
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
				return ApiResponse<WarehouseResponseDto>.Failuer(404, $"Category with Id '{id}' Not Found ");

			var response = new WarehouseResponseDto()
			{
				SerialNumber = warehouse.SerialNumber,
				WarehouseId = warehouse.WarehouseId,
			};
			return ApiResponse<WarehouseResponseDto>.Success(response, 200);
		}

		public async Task<PagedResponse<List<WarehouseResponseDto>>> GetWarehousesAsync(PaginationQueryParameters query)
		{
			var parameter = new BaseFilter()
			{
				PageNumber = query.PageNumber,
				PageSize = query.PageSize,
			};

			var (warehouses, totalCount) = await unitOfWork.WarehouseRepository.GetWarehousesWithFiltersAsync(parameter);

			if (warehouses is null )
				return PagedResponse<List<WarehouseResponseDto>>.SimpleResponse(null, 0, 0, 0,"Still No Data");

			var warehouseDtos = warehouses.Select(e => new WarehouseResponseDto()
			{
				SerialNumber = e.SerialNumber,
				WarehouseId = e.WarehouseId,
			}).ToList();

			return  PagedResponse<List<WarehouseResponseDto>>
				  .SimpleResponse(warehouseDtos, parameter.PageNumber, parameter.PageSize, totalCount);
		}

		public async Task SendProductsToWarehouse(PurchaseOrder purchaseOrder)  //////background job
		{
			var warehouse = await unitOfWork.WarehouseRepository.GetByIdAsync(purchaseOrder.WarehouseId);

			if (warehouse == null)
				throw new Exception();
			///throw new NotFoundException("Warehouse not found");

			await unitOfWork.BeginTransactionAsync();
			foreach (var item in purchaseOrder.OrderItems)
			{
				if (item.ReceivedQuantity == 0)
					continue;

				var existingInventory = warehouse.Inventories.FirstOrDefault(i => i.ProductId == item.ProductId);

				if (existingInventory != null)
					existingInventory.QuantityInStock += item.ReceivedQuantity;
				else
				{
					var newInventory = new Inventory
					{
						ProductId = item.ProductId,
						QuantityInStock = item.ReceivedQuantity,
						WarehouseId = warehouse.WarehouseId
					};

					await unitOfWork.InventoryRepository.AddAsync(newInventory);
				}
			}
			await unitOfWork.CommitTransaction();

			var recordsPurchase = purchaseOrder.OrderItems.Select(e => new RecordsPurchase
			{
				ProductId = e.ProductId,
				Quantity = e.ReceivedQuantity,
				WarehouseId = warehouse.WarehouseId
			}).ToList();
			await stockMovementServices.RecordPurchaseAsync(recordsPurchase);
		}
	}
}
