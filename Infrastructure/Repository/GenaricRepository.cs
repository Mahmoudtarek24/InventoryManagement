using Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Infrastructure.Repository
{
	public class GenaricRepository<T> : IGenaricRepository<T> where T : class
	{
		protected readonly InventoryManagementDbContext context;
		private readonly DbSet<T> entity;

		public GenaricRepository(InventoryManagementDbContext _context)
		{
			this.context = _context;
			this.entity = context.Set<T>();	
		}
		public async Task AddAsync(T entity)
		{
			await this.entity.AddAsync(entity);	
		}

		public void Delete(T entity)
		{
			this.entity.Remove(entity);
		}

		public async Task DeleteByIdAsync(int id)
		{
			var item = await entity.FindAsync(id);
			if (item != null)
			{
				entity.Remove(item);
			}
		}

		public async Task<IEnumerable<T>> GetAllEntities()
		{
			return await entity.ToListAsync();
		}

		public async Task<T?> GetByIdAsync(int id)
		{
			return await entity.FindAsync(id);
		}
		
	}
}
