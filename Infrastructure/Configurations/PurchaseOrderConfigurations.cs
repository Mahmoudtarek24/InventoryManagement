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
	public class PurchaseOrderConfigurations : IEntityTypeConfiguration<PurchaseOrder>
	{
		public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
		{
			builder.Property(e=>e.TotalCost).HasPrecision(7,2);	
			builder.HasCheckConstraint("CK_PurchaseOrder_TotalCost_Positive", "[TotalCost] >0");
			builder.HasOne(e=>e.Supplier).WithMany(e=>e.PurchaseOrders).HasForeignKey(e=>e.SupplierId);
			builder.Property(e => e.PurchaseOrderStatus).HasConversion<string>();
		}
	}
}
