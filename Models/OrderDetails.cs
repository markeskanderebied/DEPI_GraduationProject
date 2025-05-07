using DEPI_GraduationProject.Models;

public class OrderDetail
{
	public int Id { get; set; }

	public int Order_id { get; set; }
			
	public int Product_id { get; set; }

	public int Quantity { get; set; }

	public Order Order { get; set; }

	public Product Product { get; set; }
}
