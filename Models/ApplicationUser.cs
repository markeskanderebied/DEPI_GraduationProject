using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public int? Location_id { get; set; }
    public bool is_active { get; set; }
}
