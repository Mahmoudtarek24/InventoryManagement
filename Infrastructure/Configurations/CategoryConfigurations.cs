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
	public class CategoryConfigurations : IEntityTypeConfiguration<Category>
	{
		public void Configure(EntityTypeBuilder<Category> builder)
		{
			builder.Property(e => e.Name).HasMaxLength(50);
			builder.Property(e=>e.Description).HasMaxLength(150);
			builder.HasCheckConstraint("CK_DisplayOrderValue", "[DisplayOrder] > 1");
		}
	}
}
