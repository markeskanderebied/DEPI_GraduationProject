namespace DEPI_GraduationProject.Models
{
	public class SalesDetails

	{
		public int Id { get; set; }
		public int SaleId { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }

		// Navigation property
		public Sales Sales { get; set; }
		public Product Product { get; set; }
	}
}