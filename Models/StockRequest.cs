using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DEPI_GraduationProject.Models
{
    public class StockRequest
    {
        [Key]
        public int RequestId { get; set; }
        [Required]
        [ForeignKey("FromLocationId")]
        public int FromLocationId { get; set; }
        [Required]
        [ForeignKey("ToLocationId")]
        public int ToLocationId { get; set; }
        [Required]
        [ForeignKey("ProductCode")]
        public string ProductCode { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
    }
}