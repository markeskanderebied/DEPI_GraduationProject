using Microsoft.EntityFrameworkCore;
using DEPI_GraduationProject.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ApplicationUser> AspNetUsers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Inventory> Inventory { get; set; }
    public DbSet<GlassFixationCategory> glassfixationCategory { get; set; }
    public DbSet<Sales> Sales { get; set; }
    public DbSet<SaleDetails> SaleDetails { get; set; }
    public DbSet<Clients> Clients { get; set; }
    public DbSet<StockRequest> StockRequests { get; set; }

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
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .HasConstraintName("FK_Product_Category");

        modelBuilder.Entity<Sales>()
            .HasOne(s => s.Clients)
            .WithMany(c => c.Sales)
            .HasForeignKey(s => s.client_id)
            .HasConstraintName("FK_Sale_Client");

        modelBuilder.Entity<Sales>()
            .HasOne(s => s.Location)
            .WithMany()
            .HasForeignKey(s => s.location_id)
            .HasConstraintName("FK_Sale_Location");

        modelBuilder.Entity<SaleDetails>()
            .HasOne(sd => sd.Sales)
            .WithMany(s => s.SaleDetails)
            .HasForeignKey(sd => sd.sale_id)
            .HasConstraintName("FK_SaleDetail_Sale");

        modelBuilder.Entity<SaleDetails>()
            .HasOne(sd => sd.Product)
            .WithMany()
            .HasForeignKey(sd => sd.product_id)
            .HasConstraintName("FK_SaleDetail_Product");
    }
}