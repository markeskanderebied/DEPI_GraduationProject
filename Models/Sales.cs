namespace DEPI_GraduationProject.Models
{
	public class Sales

	{
		public int Id { get; set; }
		public int location_id { get; set; }
		public int client_id { get; set; }
		public DateTime sale_date { get; set; }
		
		// Navigation properties
		public required Clients Clients { get; set; }
		public required ICollection<SaleDetails> SaleDetails { get; set; }
		
		public required Location Location { get; set; }
	}
}