using Microsoft.AspNetCore.Mvc;

namespace DEPI_GraduationProject.ViewModels
{
	public class myInventoryViewModel
	{
		public string ProductCode { get; set; }
		public string LocationName { get; set; }
		public string ProductName { get; set; }
		public string ProductType { get; set; }
		public string ProductColor { get; set; }
		public int Quantity { get; set; }
		public string Shelf	{ get; set; }
		public decimal Price { get; set; }
		public decimal TotalPrice { get; set; }
	}
}
