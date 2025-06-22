using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Views
{
	public class SupplierProfileView ////to alter view you need to drop it then execute it agine 
	{
		public int SupplierId {  get; set; }	
		public string CompanyName {  get; set; }	
		public string Address {  get; set; }	
		public string? Notes {  get; set; }	
		public string? TaxDocumentPath {  get; set; }	
		public bool IsDeleted {  get; set; }	
		public DateTime? LastUpdateOn {  get; set; }	
		public DateTime CreateOn {  get; set; }	
		public bool IsVerified {  get; set; }	
		public string Email {  get; set; }	
		public string PhoneNumber {  get; set; }	
		public bool EmailConfirmed {  get; set; }	
		public int ProductCount {  get; set; }	
		public string UserId { get; set; }	
	}
}
