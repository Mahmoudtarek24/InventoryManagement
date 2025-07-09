using Domain.Enum;

namespace Application.ResponseDTO_s.PurchaseOrder
{
	public class PurchaseHistoryProductResponseDto
	{
		public int PurchaseOrderId { get; set; }
		public DateTime OrderDate { get; set; }               
		public string SupplierName { get; set; }              
		public decimal UnitPrice { get; set; }                
		public int OrderQuantity { get; set; }               
		public int ReceivedQuantity { get; set; }             
		public PurchaseOrderStatus Status { get; set; }
	}
}
