namespace DEPI_GraduationProject.DTOs
{
    public class LoginWith2faModel
    {
        public string TwoFactorCode { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public bool RememberMachine { get; set; }
    }
}