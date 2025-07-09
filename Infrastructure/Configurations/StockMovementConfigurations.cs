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
	internal class StockMovementConfigurations : IEntityTypeConfiguration<StockMovement>
	{
		public void Configure(EntityTypeBuilder<StockMovement> builder)
		{
			builder.HasKey(sm => sm.StockMovementId);
			
			builder.Property(sm => sm.MovementType).IsRequired();

			builder.Property(e => e.MovementType).HasConversion<string>();
			
			builder.Property(sm => sm.MovementDate).IsRequired();

			builder.Property(sm => sm.Quantity).IsRequired();

			builder.HasOne(sm => sm.Product).WithMany(p => p.StockMovements).HasForeignKey(sm => sm.prodcutId); 

			builder.HasOne(sm => sm.SourceWarehouse).WithMany(e=>e.DestinationStockMovements)
				         .HasForeignKey(sm => sm.SourceWarehouseId);

			builder.HasOne(sm => sm.DestinationWarehouse).WithMany(e => e.SourceStockMovements)
				   .HasForeignKey(sm => sm.DestinationWarehouseId);
		}
	}
}
