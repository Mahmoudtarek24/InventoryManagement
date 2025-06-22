using Infrastructure.Models;
using Infrastructure.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configurations
{
	public class SupplierWithUserViewConfigurations : IEntityTypeConfiguration<SupplierProfileView>
	{	
		public void Configure(EntityTypeBuilder<SupplierProfileView> builder)
		{
			builder.HasNoKey().ToView("vw_SupplierWithUser");
		}
	}
}
