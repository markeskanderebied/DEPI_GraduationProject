namespace DEPI_GraduationProject.Models
{
	public class Sales

	{
		public int Id { get; set; }
		public int location_id { get; set; }
		public int client_id { get; set; }
		public DateTime sale_date { get; set; }
		
		// Navigation properties
		public Clients Clients { get; set; }
		public ICollection<SaleDetails> SaleDetails { get; set; }
		
		public Location Location { get; set; }
	}
}