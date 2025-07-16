using Domain.Entity;
using Domain.Enum;
using Domain.Interface;
using Domain.QueryParameters;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Operation.Valid;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infrastructure.Repository
{
	public class StockMovementRepository :GenaricRepository<StockMovement> , IStockMovementRepository 
	{
		public StockMovementRepository(InventoryManagementDbContext context) : base(context) { }
		public async Task<List<StockMovement>> GetStockMovementsByProductAsync(int productId, int pageNumber) =>
				 await context.StockMovements.Include(sm => sm.Product).ThenInclude(e=>e.PurchaseOrderItems)
				 .Include(sm => sm.SourceWarehouse).Include(sm => sm.DestinationWarehouse)
				 .Where(sm => sm.prodcutId == productId).OrderByDescending(sm => sm.MovementDate)
				 .Skip((pageNumber-1)*20).Take(20).ToListAsync();

		public async Task<List<StockMovement>> GetStockMovementsByWarehouseAsync(int warehouseId, int pageNumber) =>
			 await context.StockMovements.Include(sm => sm.Product).ThenInclude(e => e.PurchaseOrderItems)
					 .Include(sm => sm.SourceWarehouse).Include(sm => sm.DestinationWarehouse)
					 .Where(sm => sm.SourceWarehouseId == warehouseId||sm.DestinationWarehouseId==warehouseId)
			         .OrderByDescending(sm => sm.MovementDate).Skip((pageNumber - 1) * 20).Take(20).ToListAsync();
		
		public async Task<(int, List<StockMovement>)> GetStockMovementsWithFiltersAsync(StockMovementFilter query)
		{
			var stockMovementQuery = context.StockMovements.AsNoTracking().Include(sm => sm.Product)
				         .Include(sm => sm.DestinationWarehouse).Include(sm => sm.SourceWarehouse).AsQueryable();

		
			if (query.WarehouseId.HasValue)
				stockMovementQuery = stockMovementQuery.Where(e => e.SourceWarehouseId == query.WarehouseId.Value ||
	                                         	e.DestinationWarehouseId == query.WarehouseId.Value);


			if (query.ProductId.HasValue)
				stockMovementQuery = stockMovementQuery.Where(sm => sm.prodcutId == query.ProductId.Value);

			if (!string.IsNullOrEmpty(query.MovementType))
			{
				var value = (StockMovementType)System.Enum.Parse(typeof(StockMovementType), query.MovementType);
				stockMovementQuery = stockMovementQuery.Where(sm => sm.MovementType == value);
			}
			if (query.DateFrom.HasValue)
				stockMovementQuery = stockMovementQuery.Where(sm => sm.MovementDate >= query.DateFrom.Value);

			if (query.DateTo.HasValue)
				stockMovementQuery = stockMovementQuery.Where(sm => sm.MovementDate <= query.DateTo.Value);


			if (!string.IsNullOrEmpty(query.searchTearm))
				stockMovementQuery = stockMovementQuery.Where(sm =>
					sm.Product.Name.Contains(query.searchTearm) ||
					sm.DestinationWarehouse.SerialNumber.Contains(query.searchTearm) ||
					sm.SourceWarehouse.SerialNumber.Contains(query.searchTearm) );

			int totalCount = await stockMovementQuery.CountAsync();

			// Apply sorting
			switch (query.StockMovementOrdering?.ToLower())
			{
				case "warehousename":
					stockMovementQuery = query.SortAscending
						? stockMovementQuery.OrderBy(e => e.SourceWarehouseId == query.WarehouseId.Value ||
												     e.DestinationWarehouseId == query.WarehouseId.Value)
						
						: stockMovementQuery.OrderByDescending(e => e.SourceWarehouseId == query.WarehouseId.Value ||
												               e.DestinationWarehouseId == query.WarehouseId.Value);
					break;
				case "quantity":
					stockMovementQuery = query.SortAscending
						? stockMovementQuery.OrderBy(sm => sm.Quantity)
						: stockMovementQuery.OrderByDescending(sm => sm.Quantity);
					break;
				default:
					// Default sorting by creation date or movement date
					stockMovementQuery = query.SortAscending
						? stockMovementQuery.OrderBy(sm => sm.MovementDate)
						: stockMovementQuery.OrderByDescending(sm => sm.MovementDate);
					break;
			}

			var result = await stockMovementQuery.Skip((query.PageNumber - 1) * query.PageSize)
				             .Take(query.PageSize).ToListAsync();
			return (totalCount, result);
		}
	}
}
