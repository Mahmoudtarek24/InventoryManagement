using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
	public class WarehouseConfigurations : IEntityTypeConfiguration<Warehouse>
	{
		public void Configure(EntityTypeBuilder<Warehouse> builder)
		{
			builder.HasKey(w => w.WarehouseId);

			builder.HasMany(w => w.purchaseOrders).WithOne(po => po.Warehouse).HasForeignKey(po => po.WarehouseId);

			builder.Property(e => e.SerialNumber).HasMaxLength(30);
		}
	}
}
