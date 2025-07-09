using Application.Constants.Enum;
using Application.DTO_s.StockMovementDto_s;
using Application.Interfaces;
using Application.ResponseDTO_s;
using Domain.Entity;
using Domain.Enum;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
	public class StockMovementServices : IStockMovementServices
	{
		private readonly IUnitOfWork unitOfWork;
		public StockMovementServices(IUnitOfWork unitOfWork)
		{
			this.unitOfWork = unitOfWork;
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> RecordSaleAsync(RecordStockMovementDto dto)
		{
			var inventory = await unitOfWork.InventoryRepository
		                          .GetInventoryByProductAndWarehouseAsync(dto.ProductId, dto.WarehouseId);

			if (inventory == null)
				throw new Exception();
			//	throw new NotFoundException("Product not found in specified warehouse.");

			if (inventory.QuantityInStock < dto.Quantity)
				throw new Exception();
			//throw new ValidationException("Insufficient stock.");

			inventory.QuantityInStock -= dto.Quantity;

			var movement = new StockMovement
			{
				prodcutId = dto.ProductId,
				Quantity = dto.Quantity,
				MovementType = StockMovementType.Sale,
				MovementDate = DateTime.Now,
				SourceWarehouseId = dto.WarehouseId,
			};

			await unitOfWork.StockMovementRepository.AddAsync(movement);
			await unitOfWork.CommitTransaction();

			var response = new ConfirmationResponseDto
			{
				Message = "Sale recorded and stock updated successfully.",
				status = ConfirmationStatus.register
			};

			return ApiResponse<ConfirmationResponseDto>.Success(response, 200);
		}

		//public async Task<ApiResponse<ConfirmationResponseDto>> RecordTransferAsync(TransferStockDto dto)
		//{
		//	var sourceInventory = await unitOfWork.InventoryRepository
		//							.GetInventoryByProductAndWarehouseAsync(dto.ProductId, dto.SourceWarehouseId);

		//	if (sourceInventory == null)
		//		throw new Exception();
		//	//throw new NotFoundException($"Product with ID {dto.ProductId} not found in source warehouse {dto.SourceWarehouseId}.");

		//	if (sourceInventory.QuantityInStock < dto.Quantity)
		//		throw new Exception();
		//		//throw new ValidationException($"Insufficient stock in source warehouse. Available: {sourceInventory.QuantityInStock}, Requested: {dto.Quantity}");


		//	var destinationInventory = await unitOfWork.InventoryRepository
		//								.GetInventoryByProductAndWarehouseAsync(dto.ProductId, dto.DestinationWarehouseId);

		//	if (destinationInventory is null)
		//	{
		//		destinationInventory = new Inventory
		//		{
		//			ProductId = dto.ProductId,
		//			WarehouseId = dto.DestinationWarehouseId,
		//			QuantityInStock = 0,
		//		};
		//		await unitOfWork.InventoryRepository.AddAsync(destinationInventory);
		//	}

		//	sourceInventory.QuantityInStock -= dto.Quantity;

		//	destinationInventory.QuantityInStock += dto.Quantity;

		//	var outboundMovement = new StockMovement
		//	{
		//		prodcutId = dto.ProductId,
		//		Quantity = dto.Quantity,
		//		MovementType = StockMovementType.TransferOut,
		//		MovementDate = DateTime.UtcNow,
		//		SourceWarehouseId = dto.SourceWarehouseId,
		//		DestinationWarehouseId = dto.DestinationWarehouseId,
		//	};

		//	var inboundMovement = new StockMovement
		//	{
		//		prodcutId = dto.ProductId,
		//		Quantity = dto.Quantity,
		//		MovementType = StockMovementType.TransferIn,
		//		MovementDate = DateTime.UtcNow,
		//		SourceWarehouseId = dto.SourceWarehouseId,
		//		DestinationWarehouseId = dto.DestinationWarehouseId,
		//	};

		//	await unitOfWork.StockMovementRepository.AddAsync(outboundMovement);
		//	await unitOfWork.StockMovementRepository.AddAsync(inboundMovement);

		//	unitOfWork.InventoryRepository.Update(sourceInventory);
		//	unitOfWork.InventoryRepository.Update(destinationInventory);

		//	await unitOfWork.CommitTransaction();

		//	// إنشاء الاستجابة
		//	var response = new ConfirmationResponseDto
		//	{
		//		Message = $"Transfer of {dto.Quantity} units completed successfully from warehouse {dto.SourceWarehouseId} to warehouse {dto.DestinationWarehouseId}.",
		//		status = ConfirmationStatus.register,
		//	};
		//}
	}
}
