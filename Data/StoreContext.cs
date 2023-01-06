using Microsoft.EntityFrameworkCore;
using Savana.Product.API.Entities;

namespace Savana.Product.API.Data;

public class StoreContext : DbContext {
    public DbSet<BrandEntity> Brands { get; set; } = null!;
    public DbSet<CategoryEntity> Categories { get; set; } = null!;
    public DbSet<ProductEntity> Products { get; set; } = null!;
    public DbSet<ProductImage> ProductImages { get; set; } = null!;
    public DbSet<PromotionEntity> Promotions { get; set; } = null!;

    public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Entity<BrandEntity>().ToTable("brands");
        builder.Entity<CategoryEntity>().ToTable("categories");
        builder.Entity<ProductEntity>().ToTable("products")
            .HasOne(p => p.Promotion)
            .WithOne(pr => pr.Product)
            .HasForeignKey<PromotionEntity>(pr => pr.ProductId);
        builder.Entity<ProductImage>().ToTable("product_images");
        builder.Entity<ProductCategory>().ToTable("product_categories")
            .HasKey(pi => new { pi.ProductId, pi.CategoryId });
        builder.Entity<PromotionEntity>().ToTable("promotions");
    }
}