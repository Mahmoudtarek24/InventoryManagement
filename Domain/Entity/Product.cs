using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
	public class Product
	{
		public int CategoryId { get; set; }
		public Category category { get; set; }	
	}
}
