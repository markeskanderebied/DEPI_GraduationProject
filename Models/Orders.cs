using System.ComponentModel.DataAnnotations;
namespace DEPI_GraduationProject.Models
{
	public class Order
	{
		public int Id { get; set; }

		[Display(Name = "From Location")]
		public int From_location { get; set; }

		[Display(Name = "To Location")]
		public int To_location { get; set; }

		public DateTime Order_date { get; set; }

		public string Status { get; set; }

		public Location From_Location { get; set; }
		public Location To_Location { get; set; }

		public List<OrderDetail> OrderDetails { get; set; }
	}
}