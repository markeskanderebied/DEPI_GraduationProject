using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DEPI_GraduationProject.ViewModels
{
	public class PosSaleViewModel
	{
		[Required]
		public string ClientName { get; set; }

		[Required]
		public string ClientNumber { get; set; }

		[Required]
		public string CarNumber { get; set; }

		public List<ProductSaleItem> Products { get; set; } = new List<ProductSaleItem>();
		public int LocationId { get; internal set; }
	}

	public class ProductSaleItem
	{
		[Required]
		public string ProductCode { get; set; }

		public string ProductName { get; set; }

		[Required]
		[Range(0.01, double.MaxValue, ErrorMessage = "Please enter a valid adhesive amount")]
		public decimal AdhesiveAmount { get; set; }
	}
}