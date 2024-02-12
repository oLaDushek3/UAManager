using Microsoft.EntityFrameworkCore;

namespace ClientLauncher.Entities;

public partial class UaClientDbContext : DbContext
{
    public UaClientDbContext()
    {
    }

    public UaClientDbContext(DbContextOptions<UaClientDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<RefundProduct> RefundProducts { get; set; }

    public virtual DbSet<UnitOfMeasurement> UnitOfMeasurements { get; set; }

    public virtual DbSet<Vat> Vats { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=212.111.84.182;Port=5432;Database=ua_client_db;Username=oladushek;Password=allgirlsarethesame");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_pk");

            entity.ToTable("employee");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.Login)
                .HasMaxLength(30)
                .HasColumnName("login");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.PostId).HasColumnName("post_id");

            entity.HasOne(d => d.Post).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_post_id_fk");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("post_pk");

            entity.ToTable("post");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("product_pk");

            entity.ToTable("product");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("money")
                .HasColumnName("price");
            entity.Property(e => e.UnitOfMeasurementId).HasColumnName("unit_of_measurement_id");
            entity.Property(e => e.VatId).HasColumnName("vat_id");

            entity.HasOne(d => d.UnitOfMeasurement).WithMany(p => p.Products)
                .HasForeignKey(d => d.UnitOfMeasurementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("product_unit_of_measurement_id_fkey");

            entity.HasOne(d => d.Vat).WithMany(p => p.Products)
                .HasForeignKey(d => d.VatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("product_vat_id_fk");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refund_pk");

            entity.ToTable("refund");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnnexDate).HasColumnName("annex_date");
            entity.Property(e => e.Customer).HasColumnName("customer");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.VoucherId).HasColumnName("voucher_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("refund_employee_id_fk");

            entity.HasOne(d => d.Voucher).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.VoucherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("refund_voucher_id_fk");
        });

        modelBuilder.Entity<RefundProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refund_product_pk");

            entity.ToTable("refund_product");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("money")
                .HasColumnName("amount");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.RefundId).HasColumnName("refund_id");

            entity.HasOne(d => d.Product).WithMany(p => p.RefundProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("refund_product_product_id_fk");

            entity.HasOne(d => d.Refund).WithMany(p => p.RefundProducts)
                .HasForeignKey(d => d.RefundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("refund_product_refund_id_fk");
        });

        modelBuilder.Entity<UnitOfMeasurement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("unit_of_measurement_pk");

            entity.ToTable("unit_of_measurement");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Vat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vat_pk");

            entity.ToTable("vat");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("voucher_pk");

            entity.ToTable("voucher");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Number).HasColumnName("number");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
