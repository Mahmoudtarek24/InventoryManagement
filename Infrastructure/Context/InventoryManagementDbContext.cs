using Domain.Entity;
using Infrastructure.Models;
using Infrastructure.Views;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
	public class InventoryManagementDbContext : IdentityDbContext<ApplicationUser>
	{
		public InventoryManagementDbContext(DbContextOptions<InventoryManagementDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly
			(
				typeof(InventoryManagementDbContext).Assembly
			);
			
			base.OnModelCreating(modelBuilder);
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
		}
		 
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }	
		public DbSet<Supplier> Supplier { get; set; }	

		//Views
		public DbSet<SupplierProfileView> SupplierProfileView { get; set; }
	}
}
