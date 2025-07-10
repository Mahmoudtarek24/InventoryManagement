using Application.ResponseDTO_s.CategoryResponse;
using Application.ResponseDTO_s.InventoryResponse;
using Application.ResponseDTO_s.ProductResponse;
using Application.ResponseDTO_s.PurchaseOrder;
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
		public static ProductResponseDto ToResponseDto(this Product product)
		{
			if (product == null)
				return null;

			return new ProductResponseDto()
			{
				CategoryId = product.CategoryId,
				CreateOn = product.CreateOn,
				Barcode = product.Barcode,
				IsAvailable = product.IsAvailable,
				IsDeleted = product.IsDeleted,
				LastUpdateOn = product.LastUpdateOn,
				Name = product.Name,
				Price = product.Price,
				ProductId = product.ProductId,
			};
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
		public static InventoryResponseDto ToResponseDto(this InventoryInfo inventory)
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
	}
}
