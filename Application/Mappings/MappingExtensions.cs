using Application.Constants.Enum;
using Application.Interfaces;
using Application.ResponseDTO_s.CategoryResponse;
using Application.ResponseDTO_s.InventoryResponse;
using Application.ResponseDTO_s.ProductResponse;
using Application.ResponseDTO_s.PurchaseOrder;
using Application.ResponseDTO_s.StockMovementResponse;
using Application.ResponseDTO_s.SupplierResponse;
using Domain.Entity;
using Domain.Enum;
using Domain.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
	public static class MappingExtensions
	{
		public static CategoryResponseDto ToResponseDto(this Category category)
		{
			if (category == null)
				return null;

			return new CategoryResponseDto()
			{
				CategoryId = category.CategoryId,
				CreateOn = category.CreateOn,
				Description = category.Description,
				DisplayOrder = category.DisplayOrder,
				IsDeleted = category.IsDeleted,
				LastUpdateOn = category.LastUpdateOn,
				Name = category.Name,
			};
		}
		public static ProductResponseDto ToResponseDto(this Product product, IUserContextService userContextService)
		{
			var response = new ProductResponseDto()
			{
				CategoryId = product.CategoryId,
				CreateOn = product.CreateOn,
				Name = product.Name,
				Price = product.Price,
				ProductId = product.ProductId,
			};
			if (userContextService.IsAdmin || userContextService.IsInventoryManager)
			{
				response.Barcode = product.Barcode;
				response.IsAvailable = product.IsAvailable;
			}
			return response;
		}
		public static ProductWithCategoryRespondDto ToResponseDtoWithCategory(this Product product)
		{
			if (product == null)
				return null;

			return new ProductWithCategoryRespondDto()
			{
				ProductId = product.ProductId,
				Name = product.Name,
				Barcode = product.Barcode,
				Price = product.Price,
				IsAvailable = product.IsAvailable,
				IsDeleted = product.IsDeleted,
				CreateOn = product.CreateOn,
				LastUpdateOn = product.LastUpdateOn,
				CategoryId = product.CategoryId,
				CategoryName = product.Category.Name
			};
		}
		public static SupplierResponseDto ToBasicResponseDto(this Supplier supplier)
		{
			if (supplier == null)
				return null;

			return new SupplierResponseDto()
			{
				IsDeleted = supplier.IsDeleted,
				CompanyName = supplier.CompanyName,
				CreateOn = supplier.CreateOn,
				IsVerified = supplier.IsVerified,
				SupplierId = supplier.SupplierId,
			};
		}
		public static SupplierVerificationStatusRespondDto ToResponseDto(this Supplier supplier)
		{
			if (supplier == null)
				return null;

			return new SupplierVerificationStatusRespondDto
			{
				Address = supplier.Address,
				CompanyName = supplier.CompanyName,
				IsVerified = supplier.IsVerified,
				Reason = supplier.RejectionReason,
				Status = supplier.VerificationStatus.ToString(),
				SupplierId = supplier.SupplierId,
			};
		}
		public static PurchaseOrderListItemResponseDto ToResponseDto(this PurchaseOrder purchaseOrder)
		{
			if (purchaseOrder == null)
				return null;

			return new PurchaseOrderListItemResponseDto
			{
				ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
				CreatedOn = purchaseOrder.CreateOn,
				PurchaseOrderId = purchaseOrder.PurchaseOrderId,
				SupplierName = purchaseOrder.Supplier.CompanyName,
				TotalCost = purchaseOrder.TotalCost,
				Status = purchaseOrder.PurchaseOrderStatus,
				TotalItems = purchaseOrder.OrderItems.Sum(e => e.OrderQuantity)
			};
		}
		public static PurchaseHistoryProductResponseDto ToResponseDto(this PurchaseOrderItem item)
		{
			if (item == null)
				return null;

			return new PurchaseHistoryProductResponseDto
			{
				PurchaseOrderId = item.PurchaseOrderId,
				OrderDate = item.PurchaseOrder.CreateOn,
				SupplierName = item.PurchaseOrder.Supplier.CompanyName,
				UnitPrice = item.UnitPrice,
				OrderQuantity = item.OrderQuantity,
				ReceivedQuantity = item.ReceivedQuantity,
				Status = item.PurchaseOrder.PurchaseOrderStatus

			};
		}
		public static InventoryResponseDto ToResponseDto(this InventoryInfo inventory)  //////شوف لو عايزا تبا تتمسح
		{
			return new InventoryResponseDto
			{
				InventoryId = inventory.InventoryId,
				ProductId = inventory.ProductId,
				ProductName = inventory.ProductName,
				QuantityInStock = inventory.QuantityInStock,
				WarehouseId = inventory.WarehouseId,
				SerialNumber = inventory.SerialNumber,
			};
		}
		public static InventoryResponseDto ToResponseDto(this Inventory inventory)
		{
			return new InventoryResponseDto
			{
				InventoryId = inventory.InventoryId,
				ProductId = inventory.ProductId,
				ProductName = inventory.Products.Name,
				QuantityInStock = inventory.QuantityInStock,
				WarehouseId = inventory.WarehouseId,
				SerialNumber = inventory.Warehouse.SerialNumber,
			};
		}
		public static LowStockAlertDto ToLowStockAlertDto(this InventoryInfo inventory)
		{
			return new LowStockAlertDto
			{
				ProductId = inventory.ProductId,
				ProductName = inventory.ProductName,
				QuantityInStock = inventory.QuantityInStock,
				WarehouseId = inventory.WarehouseId,
				SerialNumber = inventory.SerialNumber,
			};
		}
		public static PurchaseOrderBySupplierResponseDto ToResponseSupplierDto(this PurchaseOrder po)
		{
			return new PurchaseOrderBySupplierResponseDto
			{
				PurchaseOrderId = po.PurchaseOrderId,
				ExpectedDeliveryDate = po.ExpectedDeliveryDate,
				TotalCost = po.TotalCost,
				PurchaseOrderStatus = po.PurchaseOrderStatus,
				NumberOfItems = po.OrderItems?.Count ?? 0
			};
		}
		public static StockMovementResponseDto ToProductResponseDto(this StockMovement movement)
		{
			var warehouseName = GetWarehouseName(movement);
			return new StockMovementResponseDto
			{
				MovementDate = movement.MovementDate,
				MovementType = (MovementType)movement.MovementType,
				Quantity = movement.Quantity,
				ProductName = movement.Product.Name,
				WarehouseName = warehouseName,
				UnitPrice = GetUnitPrice(movement),
				SourceWarehouseName = warehouseName is null ? movement.SourceWarehouse?.SerialNumber : null,
				DestinationWarehouseName = warehouseName is null ? movement.DestinationWarehouse?.SerialNumber : null
			};
		}
		public static StockMovementResponseDto ToWarehouseResponseDto(this StockMovement movement)
		{
			return new StockMovementResponseDto
			{
				MovementDate = movement.MovementDate,
				MovementType = (MovementType)movement.MovementType,
				Quantity = movement.Quantity,
				ProductName = movement.Product.Name,
				WarehouseName = null,
				UnitPrice = GetUnitPriceForWarehouse(movement),
				SourceWarehouseName = movement.SourceWarehouse?.SerialNumber,
				DestinationWarehouseName = movement.DestinationWarehouse?.SerialNumber
			};
		}
		private static decimal? GetUnitPrice(StockMovement movement)
		{
			if (movement.MovementType != StockMovementType.ReceivedFromSupplier)
				return null;

			var item = movement.Product?.PurchaseOrderItems
				?.FirstOrDefault(e => e.ProductId == movement.prodcutId);

			return item?.UnitPrice;
		}
		private static decimal? GetUnitPriceForWarehouse(StockMovement movement)
		{
			var item = movement.Product?.PurchaseOrderItems
				?.FirstOrDefault(e => e.ProductId == movement.prodcutId);
			return item?.UnitPrice;
		}
		private static string? GetWarehouseName(StockMovement movement)
		{
			if (movement.MovementType == StockMovementType.TransferIn || movement.MovementType == StockMovementType.TransferOut)
				return null;

			return movement.SourceWarehouse.SerialNumber;
		}
	}
}

