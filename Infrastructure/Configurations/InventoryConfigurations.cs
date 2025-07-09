using Domain.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
	internal class InventoryConfigurations : IEntityTypeConfiguration<Inventory>
	{
		public void Configure(EntityTypeBuilder<Inventory> builder)
		{
			builder.HasOne(i => i.Products).WithMany(p => p.Inventories).HasForeignKey(i => i.ProductId); 

			builder.HasOne(i => i.Warehouse).WithMany(w => w.Inventories).HasForeignKey(i => i.WarehouseId);

			builder.HasIndex(i => new { i.ProductId, i.WarehouseId }).IsUnique();

			builder.Property(i => i.QuantityInStock).IsRequired();
		}
	}
}
