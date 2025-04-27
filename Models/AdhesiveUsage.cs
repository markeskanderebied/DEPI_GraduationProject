namespace DEPI_GraduationProject.Models
{
	public class AdhesiveUsage

	{
		public int Id { get; set; }
		public int SaleId { get; set; }
		public double Amount { get; set; }

		// Navigation property
		public Sales Sales { get; set; }
		public Product Product { get; set; }
		public int ProductId { get; set; }
	}
}