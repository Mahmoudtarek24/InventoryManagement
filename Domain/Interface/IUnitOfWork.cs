using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
	public interface IUnitOfWork
	{
		ICategoryRepository CategoryRepository { get; }
		IProductRepository ProductRepository { get; }
		ISupplierRepository SupplierRepository { get; }
		IPurchaseOrderItemRepository PurchaseOrderItemRepository { get; }
		IPurchaseOrderRepository PurchaseOrderRepository { get; }
		IWarehouseRepository WarehouseRepository { get; }
		IInventoryRepository InventoryRepository { get; }
		IStockMovementRepository StockMovementRepository { get; }	
		Task BeginTransactionAsync();
		Task CommitTransaction();
		Task RollbackTransaction();
		Task SaveChangesAsync();
	}
}
