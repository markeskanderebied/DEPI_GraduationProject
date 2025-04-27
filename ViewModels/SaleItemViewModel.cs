namespace DEPI_GraduationProject.ViewModels
{
	public class SaleItemViewModel
	{
		public string ProductCode { get; set; }
		public string ProductName { get; set; } // To be filled by JS on code entry
		public int Quantity { get; set; }
		public double AdhesiveAmount { get; set; }
	}
}
