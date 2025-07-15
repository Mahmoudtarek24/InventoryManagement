using Application.Constants.Enum;
using Application.DTO_s;
using Application.DTO_s.PurchaseOrder;
using Application.DTO_s.PurchaseOrderDto_s;
using Application.Exceptions;
using Application.Interfaces;
using Application.Mappings;
using Application.ResponseDTO_s;
using Application.ResponseDTO_s.PurchaseOrder;
using Application.ResponseDTO_s.SupplierResponse;
using Domain.Entity;
using Domain.Enum;
using Domain.Interface;
using Domain.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using PurchaseOrderStatus = Domain.Enum.PurchaseOrderStatus;

namespace Application.Services
{
	public class PurchaseOrderService : IPurchaseOrderService
	{
		Dictionary<string, List<PurchaseStatus>> ValidOrderStatus
					  = new Dictionary<string, List<PurchaseStatus>>()
					  {
						  {"Create",new List<PurchaseStatus>{PurchaseStatus.Draft ,PurchaseStatus.Sent} },
						  {"Update", new List<PurchaseStatus>{
													PurchaseStatus.Draft,
													PurchaseStatus.Sent,
													PurchaseStatus.Cancelled }},
						 {"ItemOperation", new List<PurchaseStatus>{ PurchaseStatus.Draft }},
					  };

		private readonly IUnitOfWork unitOfWork;
		private readonly IUserContextService userContextService;
		private readonly IUriService uriService;
		private readonly RoleBasedPurchaseOrderMapper roleBasedPurchaseOrderMapper;
		public PurchaseOrderService(IUnitOfWork unitOfWork, IUserContextService userContextService
									, RoleBasedPurchaseOrderMapper roleBasedPurchaseOrderMapper, IUriService uriService)
		{
			this.unitOfWork = unitOfWork;
			this.userContextService = userContextService;
			this.roleBasedPurchaseOrderMapper = roleBasedPurchaseOrderMapper;
			this.uriService = uriService;
		}
		public async Task<ApiResponse<ConfirmationResponseDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto)
		{
			List<string> warnings = new List<string>();

			var key = "Create";
			if (ValidOrderStatus[key].Contains((PurchaseStatus)dto.purchaseOrderStatus))
				throw new Exception();
			//throw new BadRequestException($"Invalid purchase order status '{dto.purchaseOrderStatus}' for operation '{context}'.");

			bool supplierExists = await unitOfWork.SupplierRepository.IsVerifiedAndActiveSupplierAsync(dto.SupplierId);
			if (!supplierExists)
				throw new Exception();
			//  throw new NotFoundException($"Category with ID '{dto.CategoryId}' not found.");

			var supplierProductIds = await unitOfWork.ProductRepository.GetProductsBySupplierAsync(dto.SupplierId);
			var supplierProductSet = new HashSet<int>(supplierProductIds);

			var validOrderItems = dto.purchaseOrderItemDtos
						.Where(item => supplierProductSet.Contains(item.ProductId) && item.OrderQuantity > 0).ToList();

			var invalidProductIds = dto.purchaseOrderItemDtos.Select(p => p.ProductId)
							   .Where(id => !supplierProductSet.Contains(id)).ToList();

			if (invalidProductIds.Any())
				warnings.Add($"Products with IDs [{string.Join(", ", invalidProductIds)}] are not valid for the selected supplier.");

			var purchaseOrder = new PurchaseOrder
			{
				ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
				CreateOn = DateTime.Now,
				IsDeleted = false,
				SupplierId = dto.SupplierId,
				PurchaseOrderStatus = (PurchaseOrderStatus)dto.purchaseOrderStatus,
				WarehouseId = dto.WarehouseId,
				OrderItems = new List<PurchaseOrderItem>()
			};

			var productPrices = await unitOfWork.ProductRepository
								 .GetProductPricesAsync(validOrderItems.Select(e => e.ProductId).Distinct().ToList());
			foreach (var item in validOrderItems)
			{
				purchaseOrder.OrderItems.Add(new PurchaseOrderItem
				{
					OrderQuantity = item.OrderQuantity,
					ProductId = item.ProductId,
					UnitPrice = productPrices[item.ProductId]
				});
			}
			purchaseOrder.TotalCost = purchaseOrder.OrderItems.Sum(e => e.UnitPrice * e.OrderQuantity);

			await unitOfWork.BeginTransactionAsync();
			await unitOfWork.PurchaseOrderRepository.AddAsync(purchaseOrder);
			await unitOfWork.CommitTransaction();

			ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			{
				Message = $"Purchase order for supplier #{dto.SupplierId} was successfully created with {purchaseOrder.OrderItems.Count} items. Expected delivery: {dto.ExpectedDeliveryDate:yyyy-MM-dd}.",
				status = ConfirmationStatus.Created
			};
			return ApiResponse<ConfirmationResponseDto>.SuccessWithWarnings(responseDto, 200, warnings);
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> UpdatePurchaseOrderAsync(int id, [FromBody] UpdatePurchaseOrderDto dto)
		{
			List<string> warnings = new List<string>();
			var key = "Update";

			var purchaseOrder = await unitOfWork.PurchaseOrderRepository.GetByIdAsync(id);
			if (purchaseOrder == null || purchaseOrder.IsDeleted)
				throw new Exception();
			//throw new NotFoundException($"Purchase order with ID '{id}' not found.");

			if (dto.PurchaseOrderStatus.HasValue && !ValidOrderStatus[key].Contains(dto.PurchaseOrderStatus.Value))
				throw new Exception();
			//throw new BadRequestException($"Invalid purchase order status '{dto.PurchaseOrderStatus}' for operation '{key}'.");

			await unitOfWork.BeginTransactionAsync();
			if (dto.ExpectedDeliveryDate.HasValue)
				purchaseOrder.ExpectedDeliveryDate = dto.ExpectedDeliveryDate.Value;

			if (dto.PurchaseOrderStatus.HasValue)
				purchaseOrder.PurchaseOrderStatus = (PurchaseOrderStatus)dto.PurchaseOrderStatus.Value;

			if (dto.WarehouseId.HasValue)
				purchaseOrder.WarehouseId = dto.WarehouseId.Value;

			await unitOfWork.CommitTransaction();

			ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			{
				Message = $"Purchase order #{id} was successfully updated.",
				status = ConfirmationStatus.Updated
			};

			return ApiResponse<ConfirmationResponseDto>.SuccessWithWarnings(responseDto, 200, warnings);
		}


		public async Task<ApiResponse<PurchaseOrderDetailsResponseDto>> GetPurchaseorderByIdAsync(int purchaseId)
		{
			var purchaseOrder = await unitOfWork.PurchaseOrderRepository.GetPurchaseOrderWithItemsAndSupplierAsync(purchaseId);
			if (purchaseOrder == null || purchaseOrder.IsDeleted)
				throw new Exception("Purchase order not found.");

			if (userContextService.IsSupplier)
			{
				var supplier = await unitOfWork.SupplierRepository.GetSupplierByUserIdAsync(userContextService.userId);
				if (supplier == null)
					throw new Exception("Supplier not found for current user.");

				// Check if the purchase order belongs to this supplier
				if (purchaseOrder.SupplierId != supplier.SupplierId)
					throw new Exception("Access denied. You can only view your own purchase orders.");

				var status = purchaseOrder.PurchaseOrderStatus;
				if (status != PurchaseOrderStatus.Sent &&
					status != PurchaseOrderStatus.PartiallyReceived &&
					status != PurchaseOrderStatus.Received)
				{
					throw new Exception("Access denied. You can only view purchase orders that have been sent to you.");
				}
			}

			var response = roleBasedPurchaseOrderMapper.MapToPurchaseOrderDetailsDtoAsync(purchaseOrder);
			return ApiResponse<PurchaseOrderDetailsResponseDto>.Success(response, 200);
		}

		public async Task<PagedResponse<List<PurchaseOrderListItemResponseDto>>> GetPurchaseOrdersWithPaginationAsync
												   (PurchaseOrderQueryParameter queryParam)
		{
			var filter = new PurchaseOrderFilter
			{
				PageNumber = queryParam.PageNumber,
				PageSize = queryParam.PageSize,
				searchTearm = queryParam.searchTearm,
				SortAscending = queryParam.SortAscending,
				SortBy = queryParam.SortOptions.ToString(),
				//	Status = queryParam.Status 
			};
			var (totalCount, purchaseOrders) = await unitOfWork.PurchaseOrderRepository
													   .GetPurchaseOrdersWithFiltersAsync(filter);
			if (totalCount == 0)
			{
				return PagedResponse<List<PurchaseOrderListItemResponseDto>>.SimpleResponse(
					new List<PurchaseOrderListItemResponseDto>(),
					queryParam.PageNumber,
					queryParam.PageSize,
					0,
					"No purchase orders found matching the specified criteria.");
			}

			var purchaseOrderDtos = purchaseOrders.Select(e => e.ToResponseDto()).ToList();

			var pagedResponse = PagedResponse<List<PurchaseOrderListItemResponseDto>>.SimpleResponse(purchaseOrderDtos,
				queryParam.PageNumber,
				queryParam.PageSize,
				totalCount);

			return pagedResponse.AddPagingInfo(totalCount, uriService, userContextService.Route);
		}
		public async Task<List<PurchaseOrderBySupplierResponseDto>> GetOrdersBySupplierAsync(int supplierId)
		{
			var purchaseOrders = await unitOfWork.PurchaseOrderRepository.GetPurchaseOrdersBySupplierAsync(supplierId);

			var purchaseOrderDtos = purchaseOrders.Select(po => po.ToResponseSupplierDto()).ToList();
			return purchaseOrderDtos;
		}
		public async Task<ApiResponse<ConfirmationResponseDto>> AddOrderItemAsync(int purchaseOrderId, AddOrderItemDto dto)
		{
			List<string> warnings = new List<string>();
			var key = "ItemOperation";

			var itemsToAdd = ValidateAndExtractItems(dto);
			if (!itemsToAdd.Any())
				throw new Exception();
			//	throw new BadRequestException("No valid items provided. Please provide either single item data or items list.");

			var purchaseOrder = await unitOfWork.PurchaseOrderRepository.GetPurchaseOrderWithItemsAsync(purchaseOrderId);
			if (purchaseOrder == null || purchaseOrder.IsDeleted)
				throw new Exception("Purchase order not found.");

			if (!IsValidStatusForOperation(key, (Application.Constants.Enum.PurchaseStatus)purchaseOrder.PurchaseOrderStatus))
				throw new Exception();
			//	throw new BadRequestException($"Cannot add items to purchase order with status '{purchaseOrder.PurchaseOrderStatus}'.");

			var supplierProductIds = await unitOfWork.ProductRepository.GetProductsBySupplierAsync(purchaseOrder.SupplierId);

			var existingProductIds = purchaseOrder.OrderItems.Select(item => item.ProductId).ToList();

			var validationResult = ValidateItems(itemsToAdd, supplierProductIds, existingProductIds, purchaseOrder.SupplierId);
			var validItems = validationResult.ValidItems;
			warnings.AddRange(validationResult.Warnings);

			if (!validItems.Any() && !validationResult.ItemsToUpdateQuantity.Any())
				throw new Exception();
			//throw new BadRequestException("No valid items to add after validation.");

			var productIds = validItems.Select(item => item.ProductId).Distinct().ToList();
			var productPrices = await unitOfWork.ProductRepository.GetProductPricesAsync(productIds);

			var newOrderItems = CreateOrderItems(validItems, productPrices, purchaseOrderId, warnings);

			//if (!newOrderItems.Any())
			//throw new Exception();
			//	throw new BadRequestException("No items could be added after price validation.");

			await SaveOrderItems(newOrderItems, purchaseOrder, validationResult.ItemsToUpdateQuantity);

			var responseDto = CreateResponse(newOrderItems, validationResult.ItemsToUpdateQuantity, purchaseOrderId);

			return ApiResponse<ConfirmationResponseDto>.SuccessWithWarnings(responseDto, 201, warnings);

		}

		private List<OrderItemDto> ValidateAndExtractItems(AddOrderItemDto dto)
		{
			var itemsToAdd = new List<OrderItemDto>();

			if (dto.ProductId.HasValue && dto.OrderQuantity.HasValue)
			{
				if (dto.ProductId.Value > 0 && dto.OrderQuantity.Value > 0)
				{
					itemsToAdd.Add(new OrderItemDto
					{
						ProductId = dto.ProductId.Value,
						OrderQuantity = dto.OrderQuantity.Value
					});
				}
			}

			if (dto.Items != null && dto.Items.Any())
			{
				var validListItems = dto.Items.Where(item => item.ProductId > 0 && item.OrderQuantity > 0).ToList();

				itemsToAdd.AddRange(validListItems);
			}

			var groupedItems = itemsToAdd.GroupBy(item => item.ProductId)
								.Select(group => new OrderItemDto
								{
									ProductId = group.Key,
									OrderQuantity = group.Sum(item => item.OrderQuantity)
								}).ToList();
			return groupedItems;
		}


		private ValidatePurchaseItems ValidateItems(List<OrderItemDto> itemsToAdd,
									 List<int> supplierProductIds, List<int> existingProductIds, int supplierId)
		{
			var result = new ValidatePurchaseItems();

			foreach (var item in itemsToAdd)
			{
				if (!supplierProductIds.Contains(item.ProductId))
				{
					result.Warnings.Add($"Product #{item.ProductId}: Not available from supplier #{supplierId}");
					continue;
				}

				if (existingProductIds.Contains(item.ProductId))
				{
					result.ItemsToUpdateQuantity.Add(item);
					continue;
				}

				if (item.OrderQuantity <= 0)
				{
					result.Warnings.Add($"Product #{item.ProductId}: Order quantity must be greater than zero");
					continue;
				}

				result.ValidItems.Add(item);
			}

			return result;
		}

		private List<PurchaseOrderItem> CreateOrderItems(List<OrderItemDto> validItems, Dictionary<int, decimal> productPrices,
											   int purchaseOrderId, List<string> warnings)
		{
			var newOrderItems = new List<PurchaseOrderItem>();

			foreach (var item in validItems)
			{
				if (!productPrices.ContainsKey(item.ProductId))
				{
					warnings.Add($"Product #{item.ProductId}: Price not found, skipping");
					continue;
				}

				var newOrderItem = new PurchaseOrderItem
				{
					PurchaseOrderId = purchaseOrderId,
					ProductId = item.ProductId,
					OrderQuantity = item.OrderQuantity,
					UnitPrice = productPrices[item.ProductId]
				};

				newOrderItems.Add(newOrderItem);
			}
			return newOrderItems;
		}
		private async Task SaveOrderItems(List<PurchaseOrderItem> newOrderItems, PurchaseOrder purchaseOrder, List<OrderItemDto> ItemsToIncreaseQuantity)
		{
			await unitOfWork.BeginTransactionAsync();

			decimal totalAddedCost = newOrderItems.Sum(item => item.UnitPrice * item.OrderQuantity);

			UpdateExistingItemQuantities(ItemsToIncreaseQuantity, purchaseOrder, ref totalAddedCost);
			foreach (var item in newOrderItems)
				await unitOfWork.PurchaseOrderItemRepository.AddAsync(item);

			purchaseOrder.TotalCost += totalAddedCost;
			await unitOfWork.CommitTransaction();

		}
		private void UpdateExistingItemQuantities(List<OrderItemDto> itemsToUpdate, PurchaseOrder purchaseOrder, ref decimal totalAddedCost)
		{
			if (itemsToUpdate?.Any() != true) return;

			foreach (var item in itemsToUpdate)
			{
				var existingItem = purchaseOrder.OrderItems.FirstOrDefault(e => e.ProductId == item.ProductId);
				if (existingItem != null)
				{
					existingItem.OrderQuantity += item.OrderQuantity;
					totalAddedCost += existingItem.UnitPrice * item.OrderQuantity;
				}
			}
		}
		private ConfirmationResponseDto CreateResponse(List<PurchaseOrderItem> newOrderItems,
													   List<OrderItemDto> itemsToIncreaseQuantity, int purchaseOrderId)
		{
			var messageParts = new List<string>();

			if (newOrderItems.Any())
			{
				if (newOrderItems.Count == 1)
					messageParts.Add($"Product #{newOrderItems[0].ProductId} was successfully added");
				else
				{
					var productIds = string.Join(", ", newOrderItems.Select(item => $"#{item.ProductId}"));
					messageParts.Add($"{newOrderItems.Count} products ({productIds}) were successfully added");
				}
			}

			if (itemsToIncreaseQuantity?.Any() == true)
			{
				if (itemsToIncreaseQuantity.Count == 1)
					messageParts.Add($"Product #{itemsToIncreaseQuantity[0].ProductId} quantity was increased by {itemsToIncreaseQuantity[0].OrderQuantity}");
				else
				{
					var updates = string.Join(", ", itemsToIncreaseQuantity.Select(item => $"#{item.ProductId}(+{item.OrderQuantity})"));
					messageParts.Add($"{itemsToIncreaseQuantity.Count} products had their quantities increased ({updates})");
				}
			}

			string message = string.Join(" and ", messageParts) + $" to purchase order #{purchaseOrderId}.";

			return new ConfirmationResponseDto()
			{
				Message = message,
				status = ConfirmationStatus.Created
			};
		}

		private bool IsValidStatusForOperation(string operation, PurchaseStatus status)
		{
			return ValidOrderStatus.ContainsKey(operation) && ValidOrderStatus[operation].Contains(status);
		}

		public async Task<ApiResponse<ConfirmationResponseDto>> UpdateOrderItemAsync(int purchaseOrderId, UpdateOrderItemsDto dto)
		{
			//List<string> warnings = new List<string>();
			//var key = "ItemOperation";

			//var purchaseOrder = await unitOfWork.PurchaseOrderRepository.GetByIdAsync(purchaseOrderId);
			//if (purchaseOrder == null || purchaseOrder.IsDeleted)
			//	throw new Exception();
			////throw new NotFoundException($"Purchase order with ID '{purchaseOrderId}' not found.");

			//if (!IsValidStatusForOperation(key, (Application.Constants.Enum.PurchaseStatus)purchaseOrder.PurchaseOrderStatus))
			//	throw new Exception();
			////throw new BadRequestException($"Cannot update items in purchase order with status '{purchaseOrder.PurchaseOrderStatus}'.");

			//if (dto?.Items == null || !dto.Items.Any())
			//	throw new Exception();
			////throw new BadRequestException("No items to update.");

			//var invalidItems = dto.Items.Where(item => item.ProductId <= 0 || (item.OrderQuantity.HasValue && item.OrderQuantity.Value <= 0)).ToList();
			//if (invalidItems.Any())
			//	throw new Exception();
			//	//throw new BadRequestException($"Invalid data: ProductId must be greater than 0 and OrderQuantity (if provided) must be greater than 0.");

			//var productIds = dto.Items.Select(item => item.ProductId).Distinct().ToList();
			//var productPrices = await unitOfWork.ProductRepository.GetProductPricesAsync(productIds);

			//// التحقق من وجود المنتجات في قاعدة البيانات
			//var missingProducts = productIds.Where(id => !productPrices.ContainsKey(id)).ToList();
			//if (missingProducts.Any())
			//	throw new Exception();
			////throw new BadRequestException($"Products with IDs [{string.Join(", ", missingProducts)}] not found in database.");

			//decimal totalPriceDifference = 0;
			//List<string> updatedItems = new List<string>();

			//await unitOfWork.BeginTransactionAsync();

			//foreach (var updateItem in dto.Items)
			//{
			//	// البحث عن المنتج في الطلب الحالي
			//	var orderItem = purchaseOrder.OrderItems.FirstOrDefault(item => item.ProductId == updateItem.ProductId);
			//	if (orderItem == null)
			//	{
			//		warnings.Add($"Product with ID '{updateItem.ProductId}' not found in purchase order #{purchaseOrderId}.");
			//		continue;
			//	}

			//	// حفظ القيم القديمة لحساب الفرق في التكلفة
			//	decimal oldTotalPrice = orderItem.UnitPrice * orderItem.OrderQuantity;

			//	// تحديث الكمية إذا تم تمريرها
			//	if (updateItem.OrderQuantity.HasValue)
			//	{
			//		orderItem.OrderQuantity = updateItem.OrderQuantity.Value;
			//	}

			//	// تحديث السعر من قاعدة البيانات
			//	orderItem.UnitPrice = productPrices[updateItem.ProductId];

			//	// حساب التكلفة الجديدة
			//	decimal newTotalPrice = orderItem.UnitPrice * orderItem.OrderQuantity;
			//	decimal priceDifference = newTotalPrice - oldTotalPrice;

			//	// تحديث المنتج
			//	unitOfWork.PurchaseOrderItemRepository.Update(orderItem);

			//	// إضافة الفرق في التكلفة إلى الإجمالي
			//	totalPriceDifference += priceDifference;

			//	updatedItems.Add($"Product #{updateItem.ProductId}");
			//}

			//// تحديث التكلفة الإجمالية للطلب
			//purchaseOrder.TotalCost += totalPriceDifference;
			//unitOfWork.PurchaseOrderRepository.Update(purchaseOrder);

			//await unitOfWork.CommitTransaction();

			//// إنشاء رسالة الاستجابة
			//string message;
			//if (updatedItems.Any())
			//{
			//	message = $"Order items {string.Join(", ", updatedItems)} were successfully updated in purchase order #{purchaseOrderId}.";
			//}
			//else
			//{
			//	message = $"No items were updated in purchase order #{purchaseOrderId}.";
			//}

			//// إضافة التحذيرات إلى الرسالة إذا كانت موجودة
			//if (warnings.Any())
			//{
			//	message += $" Warnings: {string.Join(" ", warnings)}";
			//}

			//ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			//{
			//	Message = message,
			//	status = updatedItems.Any() ? ConfirmationStatus.Updated : ConfirmationStatus.Warning
			//};

			//return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 200);
			return default;
		}
		public async Task<ApiResponse<ConfirmationResponseDto>> RemoveOrderItemsAsync(int purchaseOrderId, RemoveOrderItemsDto dto)
		{
			List<string> warnings = new List<string>();
			var key = "ItemOperation";

			var purchaseOrder = await unitOfWork.PurchaseOrderRepository.GetPurchaseOrderWithItemsAsync(purchaseOrderId);
			if (purchaseOrder == null || purchaseOrder.IsDeleted)
				throw new Exception();
				//throw new NotFoundException($"Purchase order with ID '{purchaseOrderId}' not found.");

			if (!IsValidStatusForOperation(key,(Application.Constants.Enum.PurchaseStatus) purchaseOrder.PurchaseOrderStatus))
				throw new Exception();
			//throw new BadRequestException($"Cannot remove items from purchase order with status '{purchaseOrder.PurchaseOrderStatus}'.");

			if (dto?.ProductIds == null || !dto.ProductIds.Any())
				throw new Exception();
			//	throw new BadRequestException("No products to remove.");

			var distinctProductIds = dto.ProductIds.Distinct().ToList();

			var orderItemsToRemove = purchaseOrder.OrderItems
				              .Where(item => distinctProductIds.Contains(item.ProductId)).ToList();

			var foundProductIds = orderItemsToRemove.Select(item => item.ProductId).ToList();
			var notFoundProductIds = distinctProductIds.Except(foundProductIds).ToList();

			if (notFoundProductIds.Any())
				warnings.AddRange(notFoundProductIds.Select(id =>
					$"Product with ID '{id}' not found in purchase order #{purchaseOrderId}."));

			
			int remainingItemsCount = purchaseOrder.OrderItems.Count - orderItemsToRemove.Count;
			if (remainingItemsCount == 0)
				throw new Exception();
			//	throw new BadRequestException("Cannot remove all items from purchase order. Consider canceling the order instead.");

			if (!orderItemsToRemove.Any())
			{
				ConfirmationResponseDto noItemsResponseDto = new ConfirmationResponseDto()
				{
					Message = $"No items were removed from purchase order #{purchaseOrderId}.",
					status = ConfirmationStatus.Warning
				};

				return ApiResponse<ConfirmationResponseDto>.SuccessWithWarnings(noItemsResponseDto, 200, warnings);
			}

			decimal totalRemovedCost = orderItemsToRemove.Sum(item => item.UnitPrice * item.OrderQuantity);

			await unitOfWork.BeginTransactionAsync();
			foreach (var orderItem in orderItemsToRemove)
			{
				unitOfWork.PurchaseOrderItemRepository.Delete(orderItem);
			}

			purchaseOrder.TotalCost -= totalRemovedCost;
			await unitOfWork.CommitTransaction();

			var removedProductIds = orderItemsToRemove.Select(item => $"Product #{item.ProductId}").ToList();

			string message = $"Order items {string.Join(", ", removedProductIds)} were successfully removed from purchase order #{purchaseOrderId}.";

			ConfirmationResponseDto responseDto = new ConfirmationResponseDto()
			{
				Message = message,
				status = ConfirmationStatus.HardDeleted
			};

			if (warnings.Any())
				return ApiResponse<ConfirmationResponseDto>.SuccessWithWarnings(responseDto, 200, warnings);
			else
				return ApiResponse<ConfirmationResponseDto>.Success(responseDto, 200);
		}
	}
}
