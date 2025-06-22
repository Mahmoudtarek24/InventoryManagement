using Domain.Entity;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
	public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			builder.Property(e => e.FullName).IsRequired().HasMaxLength(150);
			builder.Property(e => e.ProfileImage).IsRequired().HasMaxLength(250);
			builder.HasOne(e => e.Supplier).WithOne().HasForeignKey<Supplier>(e => e.UserId);
		}
	}
}
