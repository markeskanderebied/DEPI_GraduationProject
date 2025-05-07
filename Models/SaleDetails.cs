namespace DEPI_GraduationProject.Models
{
	public class SaleDetails

	{
		public int Id { get; set; }
		public int sale_id { get; set; }
		public int product_id { get; set; }
		public int Quantity { get; set; }

		// Navigation property
		public Sales Sales { get; set; }
		public Product Product { get; set; }
	}
}