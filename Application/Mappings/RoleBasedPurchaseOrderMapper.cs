using Application.Interfaces;
using Application.ResponseDTO_s.PurchaseOrder;
using Application.ResponseDTO_s.SupplierResponse;
using Domain.Entity;
using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
	public class RoleBasedPurchaseOrderMapper
	{
		private readonly IUserContextService userContextService;
		public RoleBasedPurchaseOrderMapper( IUserContextService userContextService)
		{
			this.userContextService = userContextService;
		}
		public PurchaseOrderDetailsResponseDto MapToPurchaseOrderDetailsDtoAsync(PurchaseOrder purchaseOrder)
		{
			var response = new PurchaseOrderDetailsResponseDto
			{
				PurchaseOrderId = purchaseOrder.PurchaseOrderId,
				ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
				TotalCost = purchaseOrder.TotalCost,
				PurchaseOrderStatus = purchaseOrder.PurchaseOrderStatus,
				CreatedOn = purchaseOrder.CreateOn,

				Supplier = new SupplierBaseResponsDto
				{
					SupplierId = purchaseOrder.Supplier.SupplierId,
					CompanyName = purchaseOrder.Supplier.CompanyName,
					Address = purchaseOrder.Supplier.Address,
				},

				OrderItems = purchaseOrder.OrderItems.Select(oi => new PurchaseOrderItemResponseDto
				{
					ProductId = oi.ProductId,
					ProductName = oi.Product.Name,
					UnitPrice = oi.UnitPrice,
					OrderQuantity = oi.OrderQuantity,
					ReceivedQuantity = oi.ReceivedQuantity
				}).ToList()
			};

			return response;
		}
	}
}
