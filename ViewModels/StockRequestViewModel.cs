namespace DEPI_GraduationProject.ViewModels
{
    public class StockRequestViewModel
    {
        public int RequestId { get; set; }
        public string FromLocationName { get; set; } = string.Empty;
        public string ToLocationName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}