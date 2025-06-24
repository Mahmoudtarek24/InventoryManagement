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
	public class SupplierConfigurations : IEntityTypeConfiguration<Supplier>
	{
		public void Configure(EntityTypeBuilder<Supplier> builder)
		{
			builder.Property(e => e.CompanyName).IsRequired().HasMaxLength(150);
			builder.Property(e => e.Address).IsRequired().HasMaxLength(250);
			builder.Property(e => e.Notes).HasMaxLength(10000);
			builder.Property(e => e.TaxDocumentPath).HasMaxLength(250);
			builder.Property(e => e.VerificationStatus).HasConversion<string>();
			builder.HasMany(e => e.Products).WithOne(e => e.Supplier).HasForeignKey(e => e.SupplierId);

		}
	}
}
