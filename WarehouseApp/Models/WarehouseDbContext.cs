using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace WarehouseApp.Models;

public partial class WarehouseDbContext : DbContext
{
    public WarehouseDbContext()
    {
    }

    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ExportOrder> ExportOrders { get; set; }

    public virtual DbSet<ExportOrderDetail> ExportOrderDetails { get; set; }

    public virtual DbSet<ImportOrder> ImportOrders { get; set; }

    public virtual DbSet<ImportOrderDetail> ImportOrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        IConfigurationRoot configuration = builder.Build();
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DBContext"));
        }

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2BE0F7D4E7");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<ExportOrder>(entity =>
        {
            entity.HasKey(e => e.ExportId).HasName("PK__ExportOr__E5C997A4AAFBC512");

            entity.Property(e => e.ExportId).HasColumnName("ExportID");
            entity.Property(e => e.ExportDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(200);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");

            entity.HasOne(d => d.User).WithMany(p => p.ExportOrders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ExportOrd__UserI__5070F446");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.ExportOrders)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__ExportOrd__Wareh__5165187F");
        });

        modelBuilder.Entity<ExportOrderDetail>(entity =>
        {
            entity.HasKey(e => e.ExportDetailId).HasName("PK__ExportOr__07C903599BFCBD7F");

            entity.Property(e => e.ExportDetailId).HasColumnName("ExportDetailID");
            entity.Property(e => e.ExportId).HasColumnName("ExportID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Export).WithMany(p => p.ExportOrderDetails)
                .HasForeignKey(d => d.ExportId)
                .HasConstraintName("FK__ExportOrd__Expor__5441852A");

            entity.HasOne(d => d.Product).WithMany(p => p.ExportOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ExportOrd__Produ__5535A963");
        });

        modelBuilder.Entity<ImportOrder>(entity =>
        {
            entity.HasKey(e => e.ImportId).HasName("PK__ImportOr__8697678A6B8A4162");

            entity.Property(e => e.ImportId).HasColumnName("ImportID");
            entity.Property(e => e.ImportDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(200);
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");

            entity.HasOne(d => d.Supplier).WithMany(p => p.ImportOrders)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__ImportOrd__Suppl__47DBAE45");

            entity.HasOne(d => d.User).WithMany(p => p.ImportOrders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ImportOrd__UserI__46E78A0C");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.ImportOrders)
                .HasForeignKey(d => d.WarehouseId)
                .HasConstraintName("FK__ImportOrd__Wareh__48CFD27E");
        });

        modelBuilder.Entity<ImportOrderDetail>(entity =>
        {
            entity.HasKey(e => e.ImportDetailId).HasName("PK__ImportOr__CDFBBA51A13A6CE3");

            entity.Property(e => e.ImportDetailId).HasColumnName("ImportDetailID");
            entity.Property(e => e.ImportId).HasColumnName("ImportID");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Import).WithMany(p => p.ImportOrderDetails)
                .HasForeignKey(d => d.ImportId)
                .HasConstraintName("FK__ImportOrd__Impor__4BAC3F29");

            entity.HasOne(d => d.Product).WithMany(p => p.ImportOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ImportOrd__Produ__4CA06362");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6EDB9BE3B04");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.Quantity);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__Catego__3F466844");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AA7831DEC");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666941B7DAB4D");

            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.SupplierName).HasMaxLength(200);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACD4E0739D");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4641148DE").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__RoleID__3A81B327");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.WarehouseId).HasName("PK__Warehous__2608AFD9BC9F0E75");

            entity.Property(e => e.WarehouseId).HasColumnName("WarehouseID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.WarehouseName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
