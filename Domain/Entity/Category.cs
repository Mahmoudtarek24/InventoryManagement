using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
	public class Category :BaseModel   ///business entity take execution on it
	{
		public int CategoryId { get; set; }	
		public string Name { get; set; }	
		public string? Description { get; set; }	
		public int DisplayOrder {  get; set; }	
		public ICollection<Product> Products { get; set; }
	}
}
///domain entity =>represent (كيان حقيقي) on our business mean we can add it ,modify take action based on her value 
///view  dint but on domain , becouse it used only for retrive data ,not represent real entity 
///domain should have id , but view can dint have id 
///domin only for read "dint have her behaviour" , entity execute update,delete ,create
