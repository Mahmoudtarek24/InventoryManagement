using Application.Constants.Enum;
using Application.DTO_s.StockMovementDto_s;
using Application.Interfaces;
using Application.Mappings;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.StockMovementResponse;
using Domain.Entity;
using Domain.Enum;
using Domain.Interface;
using Domain.QueryParameters;
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
		private readonly IUriService uriService;	
		private readonly IUserContextService userContextService;
		public StockMovementServices(IUnitOfWork unitOfWork,IUserContextService userContextService ,IUriService uriService)
		{
			this.unitOfWork = unitOfWork;
			this.userContextService = userContextService;
			this.uriService = uriService;
		}
		public async Task RecordPurchaseAsync(List<RecordsPurchase> dto)
		{
			await unitOfWork.BeginTransactionAsync();
			foreach (var record in dto)
			{
				var movement = new StockMovement
				{
					prodcutId = record.ProductId,
					Quantity = record.Quantity,
					MovementType = StockMovementType.ReceivedFromSupplier,
					MovementDate = DateTime.Now,
					SourceWarehouseId = record.WarehouseId,
				};
				await unitOfWork.StockMovementRepository.AddAsync(movement);
			}
			await unitOfWork.CommitTransaction();
		}
		public async Task<ApiResponse<ConfirmationResponseDto>> RecordSaleAsync(RecordStockMovementDto dtoList)
		{
			var warnings = new List<string>();
			var validatedItems = new List<(ProductsMovment dto, Inventory inventory)>();

			var productsIds = dtoList.ProductsMovments.Select(e => e.ProductId).Distinct().ToList();

			var allInventories = await unitOfWork.InventoryRepository
							  .GetInventorysByProductsAndWarehousesAsync(productsIds, new List<int> { dtoList.WarehouseId });

			var inventoryLookup = allInventories.ToDictionary(e => e.ProductId + "_" + e.WarehouseId, e => e);

			foreach (var dto in dtoList.ProductsMovments)
			{
				var key = $"{dto.ProductId}_{dtoList.WarehouseId}";

				if (!inventoryLookup.TryGetValue(key, out var inventory))
				{
					warnings.Add($"Product ID {dto.ProductId} not found in warehouse ID {dtoList.WarehouseId}");
					continue;
				}
				if (inventory.QuantityInStock < dto.Quantity)
				{
					warnings.Add($"Insufficient stock for Product ID {dto.ProductId}. Available: {inventory.QuantityInStock}, Requested: {dto.Quantity}");
					continue;
				}

				validatedItems.Add((dto, inventory));
			}
			await unitOfWork.BeginTransactionAsync();
			// Second pass: Process validated items
			foreach (var (dto, inventory) in validatedItems)
			{
				inventory.QuantityInStock -= dto.Quantity;

				var movement = new StockMovement
				{
					prodcutId = dto.ProductId,
					Quantity = dto.Quantity,
					MovementType = StockMovementType.Sale,
					MovementDate = DateTime.Now,
					SourceWarehouseId = dtoList.WarehouseId,
				};

				await unitOfWork.StockMovementRepository.AddAsync(movement);
			}
			await unitOfWork.CommitTransaction();

			var response = new ConfirmationResponseDto
			{
				Message = $"Bulk sale operation completed. {validatedItems.Count} out of {dtoList.ProductsMovments.Count} products processed successfully.",
				status = validatedItems.Count > 0 ? ConfirmationStatus.register : ConfirmationStatus.Failed
			};

			if (warnings.Any())
				return ApiResponse<ConfirmationResponseDto>.SuccessWithWarnings(response, 200, warnings);

			return ApiResponse<ConfirmationResponseDto>.Success(response, 200);
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> RecordTransferAsync(TransferStockDto dto)
		{
			var warnings = new List<string>();
			var validatedItems = new List<(ProductsMovment dto, Inventory inventory)>();
			var productIds = dto.TransferProducts.Select(e => e.ProductId).Distinct().ToList();

			var AllSourceInventory = await unitOfWork.InventoryRepository
						.GetInventorysByProductsAndWarehousesAsync(productIds, new List<int> { dto.SourceWarehouseId });

			if (AllSourceInventory is null)
				return ApiResponse<ConfirmationResponseDto>.Failuer(404, $"No product found on Warehouse With id {dto.SourceWarehouseId} To Transefer it ");

			var inventoryLookup = AllSourceInventory.ToDictionary(e => e.ProductId, e => e);

			foreach (var transferProduct in dto.TransferProducts)
			{
				if (!inventoryLookup.TryGetValue(transferProduct.ProductId, out var inventory))
				{
					warnings.Add($"Product ID {transferProduct.ProductId} not found in warehouse ID {dto.SourceWarehouseId}");
					continue;
				}
				if (inventory.QuantityInStock < transferProduct.Quantity)
				{
					warnings.Add($"Insufficient stock for Product ID {transferProduct.ProductId}. Available: {inventory.QuantityInStock}, Requested: {transferProduct.Quantity}");
					continue;
				}

				validatedItems.Add((transferProduct, inventory));   // all valid product need to add 
			}

			if (!validatedItems.Any())
			{
				var rsponse = new ConfirmationResponseDto
				{
					Message = "No valid transfers could be processed.",
					status = ConfirmationStatus.Failed
				};
				return ApiResponse<ConfirmationResponseDto>.SuccessWithWarnings(rsponse, 200, warnings);
			}

			var validProductIds = validatedItems.Select(e => e.dto.ProductId).ToList();
			var allDestInventory = await unitOfWork.InventoryRepository
				.GetInventorysByProductsAndWarehousesAsync(validProductIds, new List<int> { dto.DestinationWarehouseId });

			var DistInventoryLookup = allDestInventory.ToDictionary(e => e.ProductId, e => e);

			var totalTransferredQuantity = 0;
			await unitOfWork.BeginTransactionAsync();
			foreach (var (validSourceItem, sourceInventory) in validatedItems)
			{
				sourceInventory.QuantityInStock -= validSourceItem.Quantity;
				if (DistInventoryLookup.TryGetValue(validSourceItem.ProductId, out var destInventory))
					destInventory.QuantityInStock += validSourceItem.Quantity;

				else
				{
					var newDestInventory = new Inventory
					{
						ProductId = validSourceItem.ProductId,
						WarehouseId = dto.DestinationWarehouseId,
						QuantityInStock = validSourceItem.Quantity,
					};
					await unitOfWork.InventoryRepository.AddAsync(newDestInventory);
				}

				var outboundMovement = new StockMovement
				{
					prodcutId = validSourceItem.ProductId,
					Quantity = validSourceItem.Quantity,
					MovementType = StockMovementType.TransferOut,
					MovementDate = DateTime.UtcNow,
					SourceWarehouseId = dto.SourceWarehouseId,
					DestinationWarehouseId = dto.DestinationWarehouseId,
				};

				var inboundMovement = new StockMovement
				{
					prodcutId = validSourceItem.ProductId,
					Quantity = validSourceItem.Quantity,
					MovementType = StockMovementType.TransferIn,
					MovementDate = DateTime.UtcNow,
					SourceWarehouseId = dto.SourceWarehouseId,
					DestinationWarehouseId = dto.DestinationWarehouseId,
				};

				await unitOfWork.StockMovementRepository.AddAsync(outboundMovement);
				await unitOfWork.StockMovementRepository.AddAsync(inboundMovement);
				
				totalTransferredQuantity += validSourceItem.Quantity;
			}
			await unitOfWork.CommitTransaction();

			var response = new ConfirmationResponseDto
			{
				Message = $"Transfer of  units completed successfully from warehouse {dto.SourceWarehouseId} to warehouse {dto.DestinationWarehouseId}.",
				status = ConfirmationStatus.register,
			};
			
			var successResponse = new ConfirmationResponseDto
			{
				Message = $"Transfer of {totalTransferredQuantity} units completed successfully from warehouse {dto.SourceWarehouseId} to warehouse {dto.DestinationWarehouseId}",
				status = ConfirmationStatus.register,
			};

			if (warnings.Any())
				return ApiResponse<ConfirmationResponseDto>.SuccessWithWarnings(successResponse, 200, warnings);

			return ApiResponse<ConfirmationResponseDto>.Success(successResponse, 200);
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> RecordAdjustmentAsync(AdjustmentDto dto)
		{
			var warnings = new List<string>();
			var validatedItems = new List<(ProductsMovment dto, Inventory inventory)>();

			var productIds = dto.ProductsMovments.Select(e => e.ProductId).Distinct().ToList();

			var allInventories = await unitOfWork.InventoryRepository
							  .GetInventorysByProductsAndWarehousesAsync(productIds, new List<int> { dto.WarehouseId });

			var inventoryLookup = allInventories.ToDictionary(e => e.ProductId, e => e);

			foreach (var productMovement in dto.ProductsMovments)
			{
				if (!inventoryLookup.TryGetValue(productMovement.ProductId, out var inventory))
				{
					warnings.Add($"Product ID {productMovement.ProductId} not found in warehouse ID {dto.WarehouseId}");
					continue;
				}

				// For negative adjustments, check if we have enough stock
				if (productMovement.Quantity < 0 && inventory.QuantityInStock < Math.Abs(productMovement.Quantity))
				{
					warnings.Add($"Insufficient stock for Product ID {productMovement.ProductId}. Available: {inventory.QuantityInStock}, Requested adjustment: {productMovement.Quantity}");
					continue;
				}

				validatedItems.Add((productMovement, inventory));
			}

			if (!validatedItems.Any())
			{
				var failedResponse = new ConfirmationResponseDto
				{
					Message = "No valid adjustments could be processed.",
					status = ConfirmationStatus.Failed
				};
				return ApiResponse<ConfirmationResponseDto>.SuccessWithWarnings(failedResponse, 200, warnings);
			}

			await unitOfWork.BeginTransactionAsync();

			var totalAdjustedQuantity = 0;

			foreach (var (productMovement, inventory) in validatedItems)
			{
				// Update inventory quantity
				inventory.QuantityInStock += productMovement.Quantity;

				// Create stock movement record
				var movement = new StockMovement
				{
					prodcutId = productMovement.ProductId,
					Quantity = Math.Abs(productMovement.Quantity), // Store absolute value
					MovementType = productMovement.Quantity > 0 ? StockMovementType.ManualAdjustmentIncrease : StockMovementType.ManualAdjustmentDecrease,
					MovementDate = DateTime.UtcNow,
					SourceWarehouseId = dto.WarehouseId,
				};

				await unitOfWork.StockMovementRepository.AddAsync(movement);

				totalAdjustedQuantity += Math.Abs(productMovement.Quantity);
			}

			await unitOfWork.CommitTransaction();

			var successResponse = new ConfirmationResponseDto
			{
				Message = $"Adjustment of {totalAdjustedQuantity} units completed successfully in warehouse {dto.WarehouseId}. {validatedItems.Count} out of {dto.ProductsMovments.Count} products processed successfully.",
				status = ConfirmationStatus.register
			};

			if (warnings.Any())
				return ApiResponse<ConfirmationResponseDto>.SuccessWithWarnings(successResponse, 200, warnings);

			return ApiResponse<ConfirmationResponseDto>.Success(successResponse, 200);

		}

		public async Task<ApiResponse<List<StockMovementResponseDto>>>GetMovementsByProductAsync(int productId,int pageNumber)
		{
			var stockMovements = await unitOfWork.StockMovementRepository
				                            .GetStockMovementsByProductAsync(productId, pageNumber);

			if (!stockMovements.Any())
				return ApiResponse<List<StockMovementResponseDto>>.Success(null,
					                         200, "No transactions found for this product");

			var responseDtos = stockMovements.Select(movement => movement.ToProductResponseDto()).ToList();
			return ApiResponse<List<StockMovementResponseDto>>.Success(responseDtos, 200);
		}

		public async Task<ApiResponse<List<StockMovementResponseDto>>> GetMovementsByWarehouseAsync(int warehouseId, int pageNumber)
		{
			var warehousMovements = await unitOfWork.StockMovementRepository
											.GetStockMovementsByWarehouseAsync(warehouseId, pageNumber);
			if (!warehousMovements.Any())
				return ApiResponse<List<StockMovementResponseDto>>.Success(null,
											 200, "No transactions found for this Warehouse");

			var responseDtos = warehousMovements.Select(movement => movement.ToWarehouseResponseDto()).ToList();
			return ApiResponse<List<StockMovementResponseDto>>.Success(responseDtos, 200);
		}

		public async Task<PagedResponse<List<StockMovementResponseDto>>> GetStockMovementsAsync(StockMovementQueryParameters query)
		{
			var filter = new StockMovementFilter
			{
				PageNumber = query.PageNumber,
				PageSize = query.PageSize,
				searchTearm = query.searchTearm,
				SortAscending = query.SortAscending,
				SortBy = query.StockMovementOrdering.ToString(),
				ProductId = query.ProductId,
				MovementType = query.MovementType.ToString(),
				DateFrom = query.DateFrom,
				DateTo = query.DateTo
			};

			var (totalCount, stockMovements) = await unitOfWork.StockMovementRepository
				                                     .GetStockMovementsWithFiltersAsync(filter);

			if (totalCount == 0)
				return PagedResponse<List<StockMovementResponseDto>>.SimpleResponse(null,query.PageNumber,query.PageSize,0
					, "No stock movements found matching the specified criteria.");

		
			var stockMovementDtos = stockMovements.Select(e => e.ToWarehouseResponseDto()).ToList();
			var pagedResponse = PagedResponse<List<StockMovementResponseDto>>.SimpleResponse( stockMovementDtos,
				query.PageNumber, query.PageSize, totalCount);

			return pagedResponse.AddPagingInfo(totalCount, uriService, userContextService.Route);
		}
	}
}
