
namespace DEPI_GraduationProject.Models
{
	public class Clients
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string car_number { get; set; }
		//public string CarModel { get; set; }

		// Navigation property
		public ICollection<Sales> Sales { get; set; }
	}
}