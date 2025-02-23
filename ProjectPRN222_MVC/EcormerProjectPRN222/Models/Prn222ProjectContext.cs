using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EcormerProjectPRN222.Models;

public partial class Prn222ProjectContext : DbContext
{
    public Prn222ProjectContext()
    {
    }

    public Prn222ProjectContext(DbContextOptions<Prn222ProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attribute> Attributes { get; set; }

    public virtual DbSet<AttributeValue> AttributeValues { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Variant> Variants { get; set; }

    public virtual DbSet<VariantAttribute> VariantAttributes { get; set; }

    public virtual DbSet<VarriantStock> VarriantStocks { get; set; }
    private string? GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DBDefault"];
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attribute>(entity =>
        {
            entity.ToTable("attributes");

            entity.Property(e => e.AttributeId).HasColumnName("attribute_id");
            entity.Property(e => e.AttributeName).HasColumnName("attribute_name");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<AttributeValue>(entity =>
        {
            entity.HasKey(e => e.ValueId);

            entity.ToTable("attribute_values");

            entity.Property(e => e.ValueId).HasColumnName("value_id");
            entity.Property(e => e.AttributeId).HasColumnName("attribute_id");
            entity.Property(e => e.ValueName).HasColumnName("value_name");

            entity.HasOne(d => d.Attribute).WithMany(p => p.AttributeValues)
                .HasForeignKey(d => d.AttributeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_attribute_values_attributes");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK_brand");

            entity.ToTable("brands");

            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.BrandName).HasColumnName("brand_name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Img).HasColumnName("img");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK_cart");

            entity.ToTable("carts");

            entity.Property(e => e.CartId).HasColumnName("cart_id");
            entity.Property(e => e.AddedAd)
                .HasColumnType("datetime")
                .HasColumnName("added_ad");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_carts_users");

            entity.HasOne(d => d.Variant).WithMany(p => p.Carts)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_carts_variants");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_category");

            entity.ToTable("categories");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName).HasColumnName("category_name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Img).HasColumnName("img");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.ToTable("discounts");

            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.DiscountType).HasColumnName("discount_type");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.MinOrderValue).HasColumnName("min_order_value");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK_order");

            entity.ToTable("orders");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.DateBy)
                .HasColumnType("datetime")
                .HasColumnName("dateBy");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_payment");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_user");

            entity.HasMany(d => d.Discounts).WithMany(p => p.Orders)
                .UsingEntity<Dictionary<string, object>>(
                    "OrderDiscount",
                    r => r.HasOne<Discount>().WithMany()
                        .HasForeignKey("DiscountId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_order_discounts_discounts"),
                    l => l.HasOne<Order>().WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_order_discounts_orders"),
                    j =>
                    {
                        j.HasKey("OrderId", "DiscountId");
                        j.ToTable("order_discounts");
                        j.IndexerProperty<int>("OrderId")
                            .ValueGeneratedOnAdd()
                            .HasColumnName("order_id");
                        j.IndexerProperty<int>("DiscountId").HasColumnName("discount_id");
                    });
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK_orderDetail");

            entity.ToTable("orderDetails");

            entity.Property(e => e.OrderDetailId)
                .ValueGeneratedNever()
                .HasColumnName("orderDetail_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orderDetail_order");

            entity.HasOne(d => d.Variant).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orderDetail_variants");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK_payment");

            entity.ToTable("payments");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.PaymentName).HasColumnName("payment_name");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_product");

            entity.ToTable("products");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Img).HasColumnName("img");
            entity.Property(e => e.ProductName).HasColumnName("product_name");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Views).HasColumnName("views");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_product_brand");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_product_category");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK_role");

            entity.ToTable("roles");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.RoleName).HasColumnName("role_name");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_user");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("dob");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("user_name");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_user_role");
        });

        modelBuilder.Entity<Variant>(entity =>
        {
            entity.HasKey(e => e.VarriantId);

            entity.ToTable("variants");

            entity.Property(e => e.VarriantId).HasColumnName("varriant_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("sku");

            entity.HasOne(d => d.Product).WithMany(p => p.Variants)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_variants_product");
        });

        modelBuilder.Entity<VariantAttribute>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("variant_attribute");

            entity.Property(e => e.AttributeId).HasColumnName("attribute_id");
            entity.Property(e => e.ValueId).HasColumnName("value_id");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.Attribute).WithMany()
                .HasForeignKey(d => d.AttributeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_variant_attribute_attributes");

            entity.HasOne(d => d.Value).WithMany()
                .HasForeignKey(d => d.ValueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_variant_attribute_attribute_values");

            entity.HasOne(d => d.Variant).WithMany()
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_variant_attribute_variants");
        });

        modelBuilder.Entity<VarriantStock>(entity =>
        {
            entity.HasKey(e => e.StockId);

            entity.ToTable("varriant_stock");

            entity.Property(e => e.StockId).HasColumnName("stock_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.VariantId).HasColumnName("variant_id");

            entity.HasOne(d => d.Variant).WithMany(p => p.VarriantStocks)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_varriant_stock_variants");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
