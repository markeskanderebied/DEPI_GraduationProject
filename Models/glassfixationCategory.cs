﻿// File: Models/Inventory.cs
using System.ComponentModel.DataAnnotations.Schema;


namespace DEPI_GraduationProject.Models
{
	public class GlassFixationCategory{
		public int id { get; set; }
		public string CatName { get; set; }
		public decimal fixation_cost { get; set; }

		public ICollection<Product> Products { get; set; } // Add this line
	}


}
