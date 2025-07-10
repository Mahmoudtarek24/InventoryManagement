using Application.DTO_s;
using Application.Interfaces;
using Application.Mappings;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.InventoryResponse;
using Domain.Entity;
using Domain.Interface;
using Domain.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
	public class InventoryService : IInventoryService
	{
		private readonly IUnitOfWork unitOfWork;
		public InventoryService(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}
		public async Task<ApiResponse<InventoryResponseDto>> GetInventoryByProductAndWarehouseAsync(int productId, int warehouseId)
		{
			var inventory = await unitOfWork.InventoryRepository
								.GetInventoryByProductAndWarehouseAsync(productId, warehouseId);

			if (inventory is null)
				return ApiResponse<InventoryResponseDto>.Success(null
						 , 200, "Inventory not found for the specified product and warehouse");

			var inventoryDto = inventory.ToResponseDto();
			return ApiResponse<InventoryResponseDto>.Success(inventoryDto, 200);
		}

		public async Task<PagedResponse<List<InventoryResponseDto>>> GetInventoryByWarehouseAsync(BaseQueryParameters query, int warehouseId)
		{
			var warehouseExists = await unitOfWork.WarehouseRepository.ExistsAsync(warehouseId);
			if (!warehouseExists)
				throw new Exception(); //"Warehouse not found"

			var parameter = new BaseFilter()
			{   
				PageNumber = query.PageNumber,
				PageSize = query.PageSize,
				searchTearm = query.searchTearm,
			};

			var (inventoryList, totalCount) = await unitOfWork.InventoryRepository.GetInventoryByWarehouseWithFiltersAsync(warehouseId, parameter);

			if (inventoryList == null || !inventoryList.Any())
				return PagedResponse<List<InventoryResponseDto>>
					 .SimpleResponse(new List<InventoryResponseDto>(),0,0,0);

		//	query.TotalCount = totalCount;	
			var inventoryDto = inventoryList.Select(e=>e.ToResponseDto()).ToList();

			return PagedResponse<List<InventoryResponseDto>>
				      .SimpleResponse(inventoryDto, parameter.PageNumber, parameter.PageSize, totalCount);
		}

		public async Task<ApiResponse<List<InventoryResponseDto>>> GetInventoryByProductAsync(int productId)
		{
			var inventoryList = await unitOfWork.InventoryRepository.GetInventoryByProductAsync(productId);

			if (!inventoryList.Any())
				return ApiResponse<List<InventoryResponseDto>>.Success(null
						 , 200, "Inventory not found for the specified product ");


			var inventoryDto = inventoryList.Select(e => e.ToResponseDto()).ToList();

			return ApiResponse<List<InventoryResponseDto>>.Success(inventoryDto, 200);	
		}

		public async Task<ApiResponse<List<LowStockAlertDto>>> GetLowStockAlertsAsync(int threshold)
		{
			var lowStockItems = await unitOfWork.InventoryRepository.GetLowStockItemsAsync(threshold);

			if (!lowStockItems.Any())
				return ApiResponse<List<LowStockAlertDto>>.Success(null,
					200, "No low stock items found");

			var lowStockDto = lowStockItems.Select(e => e.ToLowStockAlertDto()).ToList();
			return ApiResponse<List<LowStockAlertDto>>.Success(lowStockDto, 200);
		}
	}
}


