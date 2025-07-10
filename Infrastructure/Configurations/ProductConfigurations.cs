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
	public class ProductConfigurations : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.Property(e => e.Name).HasMaxLength(150);
			builder.Property(e=>e.Barcode).HasMaxLength(100);
			builder.Property(e => e.Price).HasPrecision(9, 3);
			builder.HasCheckConstraint("CK_Price_Positive", "[Price] >0");
			builder.HasOne(e=>e.Category).WithMany(e=>e.Products).HasForeignKey(e=>e.CategoryId);
			builder.HasIndex(e => new { e.Name, e.CategoryId }).HasFilter("[IsDeleted] =0");

		}
	}
}
