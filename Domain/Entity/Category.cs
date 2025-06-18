using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
	public class Category :BaseModel
	{
		public int CategoryId { get; set; }	
		public string Name { get; set; }	
		public string? Description { get; set; }	
		public int DisplayOrder {  get; set; }	
		public ICollection<Product> Products { get; set; }
	}
}
