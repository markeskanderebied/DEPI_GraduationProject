using Microsoft.EntityFrameworkCore;
using DEPI_GraduationProject.Models;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<Employee> Employees { get; set; }
	public DbSet<Location> Locations { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<Inventory> Inventory { get; set; }
	public DbSet<glassfixationCategory> glassfixationCategory { get; set; }
	public DbSet<Sales> Sales { get; set; }
	public DbSet<SalesDetails> SalesDetails { get; set; }
	public DbSet<Clients> Clients { get; set; }
	//public DbSet<AdhesiveUsage> AdhesiveUsage { get; set; }
	public object Product { get; internal set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Inventory>()
			.HasOne(i => i.Product)
			.WithMany()
			.HasForeignKey(i => i.Product_id)
			.HasConstraintName("FK_Inventory_Product");

		modelBuilder.Entity<Inventory>()
			.HasOne(i => i.Location)
			.WithMany()
			.HasForeignKey(i => i.Location_id)
			.HasConstraintName("FK_Inventory_Location");

		modelBuilder.Entity<Product>()
		   .HasOne(p => p.Category)
		   .WithMany()
		   .HasForeignKey(p => p.CategoryId)
		   .HasConstraintName("FK_Product_Category");

		// Sales - Clients relationship
		modelBuilder.Entity<Sales>()
			.HasOne(s => s.Clients)
			.WithMany(c => c.Sales)
			.HasForeignKey(s => s.ClientId)
			.HasConstraintName("FK_Sale_Client");

		// Sales - Location relationship
		modelBuilder.Entity<Sales>()
			.HasOne(s => s.Location)
			.WithMany()
			.HasForeignKey(s => s.LocationId)
			.HasConstraintName("FK_Sale_Location");

		// SalesDetails - Sales relationship
		modelBuilder.Entity<SalesDetails>()
			.HasOne(sd => sd.Sales)
			.WithMany(s => s.SaleDetails)
			.HasForeignKey(sd => sd.SaleId)
			.HasConstraintName("FK_SaleDetail_Sale");

		// SalesDetails - Product relationship
		modelBuilder.Entity<SalesDetails>()
			.HasOne(sd => sd.Product)
			.WithMany()
			.HasForeignKey(sd => sd.ProductId)
			.HasConstraintName("FK_SaleDetail_Product");
		/*
		// AdhesiveUsage - Sales relationship
		modelBuilder.Entity<AdhesiveUsage>()
			.HasOne(au => au.Sales)
			.WithMany(s => s.AdhesiveUsages)
			.HasForeignKey(au => au.SaleId)
			.HasConstraintName("FK_AdhesiveUsage_Sale");*/

		/*
		// AdhesiveUsage - Product relationship
		modelBuilder.Entity<AdhesiveUsage>()
			.HasOne(au => au.Product)
			.WithMany()
			.HasForeignKey(au => au.ProductId)
			.HasConstraintName("FK_AdhesiveUsage_Product");
		*/
	}
}