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
	public class PurchaseOrderItemConfigurations : IEntityTypeConfiguration<PurchaseOrderItem>
	{
		public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
		{
			builder.HasKey(e => e.PurchaseOrderItemId);
			builder.Property(e => e.OrderQuantity).IsRequired();
			builder.Property(e => e.ReceivedQuantity).HasDefaultValue(0);
			builder.Property(e => e.UnitPrice).HasPrecision(6, 2);
			builder.HasCheckConstraint("CK_PurchaseOrderItem_OrderQuantity_Positive", "[OrderQuantity] > 0");

			builder.HasOne(e => e.Product)
				.WithMany(p => p.PurchaseOrderItems)
				.HasForeignKey(e => e.ProductId);
		}
	}
}
