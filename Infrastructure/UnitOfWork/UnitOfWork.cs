using Domain.Interface;
using Infrastructure.Context;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore.Storage;
 
namespace Infrastructure.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork, IDisposable
	{
		private readonly InventoryManagementDbContext context;
		private IDbContextTransaction? objTrans = null;
		public ICategoryRepository CategoryRepository {  get; private set; }
		public IProductRepository ProductRepository { get; private set; }
		public ISupplierRepository SupplierRepository { get; private set; }
		public IPurchaseOrderItemRepository PurchaseOrderItemRepository { get; private set; }
		public IPurchaseOrderRepository PurchaseOrderRepository { get; private set; }

		public UnitOfWork(InventoryManagementDbContext context)
		{
			this.context = context;
			CategoryRepository =new CategoryRepository(context);	
			ProductRepository =new ProductRepository(context);
			SupplierRepository =new SupplierRepository(context);
			PurchaseOrderItemRepository=new PurchaseOrderItemRepository(context);
			PurchaseOrderRepository=new PurchaseOrderRepository(context);	
		}
		public async Task BeginTransactionAsync()
		{
			objTrans = await context.Database.BeginTransactionAsync();
		}

		public async Task CommitTransaction()
		{
			try
			{
				await context.SaveChangesAsync();
				if (objTrans != null)
					await objTrans.CommitAsync();
			}
			catch
			{
				await RollbackTransaction();
				throw;  /////this is very important that will go to execute middlware excepthion // bigger try catch handle it 
			}
			finally
			{
				await DisposeTransaction();
			}
		}

		public async Task RollbackTransaction()
		{
			try
			{
				if (objTrans != null)
					await objTrans.RollbackAsync();
			}
			finally
			{
				await DisposeTransaction();
			}
		}

		public async Task SaveChangesAsync()
		{
			await context.SaveChangesAsync();
		}

		public void Dispose()
		{
			objTrans?.Dispose();
			context.Dispose();
		}
		private async Task DisposeTransaction()
		{
			if (objTrans != null)
			{
				await objTrans.DisposeAsync();
				objTrans=null;	
			}
		}
	}
}
