namespace DEPI_GraduationProject.ViewModels
{
	public class BatchSalesEntryViewModel
	{
		public int ClientId { get; set; }
		public string ClientName { get; set; }
		public string ClientNumber { get; set; }
		public string CarNumber { get; set; }

		public List<SaleItemViewModel> Orders { get; set; } = new();
	}
}
