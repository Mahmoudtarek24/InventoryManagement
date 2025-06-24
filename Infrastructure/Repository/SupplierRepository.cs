using Domain.Entity;
using Domain.Interface;
using Domain.Parameters;
using Domain.QueryParameters;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
	public class SupplierRepository :GenaricRepository<Supplier> , ISupplierRepository
	{
		public SupplierRepository(InventoryManagementDbContext context):base(context) { }

		public async Task<bool> IsCompanyNameExistsAsync(string companyName ) =>
	               await context.Supplier.AnyAsync(e=>e.CompanyName == companyName);

		public async Task<(int,List<SupplierInfo>)> SupplierWithProductCountAsync(BaseFilter filter)////////////////////
		{
			var query = context.SupplierProfileView.AsQueryable();

			// Search functionality
			if (!string.IsNullOrEmpty(filter.searchTearm))
				query = query.Where(e =>
					e.CompanyName.Contains(filter.searchTearm, StringComparison.OrdinalIgnoreCase) ||
					e.Email.Contains(filter.searchTearm, StringComparison.OrdinalIgnoreCase) ||
					e.PhoneNumber.Contains(filter.searchTearm, StringComparison.OrdinalIgnoreCase));


			int totalCount = await query.CountAsync();

			// Sorting functionality
			if (!string.IsNullOrEmpty(filter.SortBy))
			{
				switch (filter.SortBy.ToLower())
				{
					case "companyname":
						query = filter.SortAscending ? query.OrderBy(e => e.CompanyName) : query.OrderByDescending(e => e.CompanyName);
						break;
					case "isverified":
						query = filter.SortAscending ? query.OrderBy(e => e.IsVerified) : query.OrderByDescending(e => e.IsVerified);
						break;
					case "email":
						query = filter.SortAscending ? query.OrderBy(e => e.Email) : query.OrderByDescending(e => e.Email);
						break;
					case "phonenumber":
						query = filter.SortAscending ? query.OrderBy(e => e.PhoneNumber) : query.OrderByDescending(e => e.PhoneNumber);
						break;
					case "productcount":
						query = filter.SortAscending ? query.OrderBy(e => e.ProductCount) : query.OrderByDescending(e => e.ProductCount);
						break;
					default:
						query = filter.SortAscending ? query.OrderBy(e => e.CreateOn) : query.OrderByDescending(e => e.CreateOn);
						break;
				}
			}

			query = query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

			var result = await query.ToListAsync();
			var suppliers = result.Select(e=>new SupplierInfo()
			{
				Address = e.Address,
				CompanyName = e.CompanyName,
				CreateOn = e.CreateOn,
				Email = e.Email,	
				EmailConfirmed = e.EmailConfirmed,
				IsDeleted = e.IsDeleted,
				IsVerified = e.IsVerified,
				LastUpdateOn = e.LastUpdateOn,	
				Notes = e.Notes,
				PhoneNumber = e.PhoneNumber,
				ProductCount = e.ProductCount,
				SupplierId=e.SupplierId,	
				TaxDocumentPath = e.TaxDocumentPath,
				userId = e.UserId,	
			}).ToList();
			return (totalCount, suppliers);
		}


		public async  Task<bool> IsCompanyNameExistsAsync(string companyName, int? excludeId = null)
		{
			var query = context.Supplier.Where(s => !s.IsDeleted &&
													s.CompanyName.Equals(companyName, StringComparison.OrdinalIgnoreCase));
			if (excludeId.HasValue)
			{
				query = query.Where(s => s.SupplierId != excludeId.Value);
			}
			return await query.AnyAsync();
		}
		public async Task<List<Supplier>> GetSuppliersByVerificationStatusAsync(bool? isVerified = null)
		{
			var query = context.Supplier.Where(s => !s.IsDeleted);

			if (isVerified.HasValue)
			{
				query = query.Where(s => s.IsVerified == isVerified.Value);
			}
			query = query.OrderBy(s => s.CompanyName);

			return await query.ToListAsync();
		}

		//public async override Task<IEnumerable<Supplier>> GetAllEntities()
		//{
		//	return await context.Supplier.Include(e=>e.Products).ToListAsync();
		//}
	}
}
