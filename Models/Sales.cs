namespace DEPI_GraduationProject.Models
{
	public class Sales

	{
		public int Id { get; set; }
		public int LocationId { get; set; }
		public int ClientId { get; set; }
		public DateTime SaleDate { get; set; }
		
		// Navigation properties
		public Clients Clients { get; set; }
		public ICollection<SalesDetails> SaleDetails { get; set; }
		public ICollection<AdhesiveUsage> AdhesiveUsages { get; set; }
		public Location Location { get; set; }
	}
}