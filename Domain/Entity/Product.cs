using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
	public class Product :BaseModel
	{
		public int ProductId { get; set; }	
		public string Name { get; set; }	
		public string Barcode { get; set; }	
		public decimal Price { get; set; }	
		public bool IsAvailable { get; set; }	
		public int CategoryId { get; set; }
		public Category Category { get; set; }	
	}
}
