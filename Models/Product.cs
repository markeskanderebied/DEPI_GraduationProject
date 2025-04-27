namespace DEPI_GraduationProject.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string name { get; set; }
		public decimal Price { get; set; }
		public String color { get; set; }
		public string type { get; set; }
		public string Code { get; set; }

		public int CategoryId { get; set; }  // Foreign key to GlassFixationCategory
		public glassfixationCategory Category { get; set; }  // Navigation property




	}
}
