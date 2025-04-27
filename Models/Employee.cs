public class Employee
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Phone { get; set; }
	public string Username { get; set; }
	public string Password_hash { get; set; }
	public string Role { get; set; }
	public int? Location_id { get; set; }
	public bool is_active { get; set; }
}
