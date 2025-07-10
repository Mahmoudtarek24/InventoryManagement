using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface
{
	public interface IGenaricRepository<T> where T : class
	{
		Task<T?> GetByIdAsync(int id);
		Task<IEnumerable<T>> GetAllEntities();
		Task AddAsync(T entity);
		void Delete(T entity);
		Task DeleteByIdAsync(int id);
		Task AddRangeAsync(IEnumerable<T> entities);
	}
}
