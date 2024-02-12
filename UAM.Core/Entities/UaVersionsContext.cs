using Microsoft.EntityFrameworkCore;

namespace UAM.Core.Entities;

public partial class UaVersionsContext : DbContext
{
    public UaVersionsContext()
    {
    }

    public UaVersionsContext(DbContextOptions<UaVersionsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dependency> Dependencies { get; set; }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<Problem> Problems { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Version> Versions { get; set; }

    public virtual DbSet<VersionDependency> VersionDependencies { get; set; }
    
    public virtual DbSet<Worker> Workers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=212.111.84.182;Port=5432;Database=ua_versions;Username=oladushek;Password=allgirlsarethesame");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("priority_pkey");

            entity.ToTable("priority");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Problem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("problem_pkey");

            entity.ToTable("problem");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedTime)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_time");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.PriorityId).HasColumnName("priority_id");
            entity.Property(e => e.ProblemText).HasColumnName("problem_text");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.WorkerId).HasColumnName("worker_id");

            entity.HasOne(d => d.Priority).WithMany(p => p.Problems)
                .HasForeignKey(d => d.PriorityId)
                .HasConstraintName("problem_priority_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Problems)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("problem_status_id_fkey");

            entity.HasOne(d => d.Worker).WithMany(p => p.Problems)
                .HasForeignKey(d => d.WorkerId)
                .HasConstraintName("problem_worker_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("status_pkey");

            entity.ToTable("status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Worker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("worker_pkey");

            entity.ToTable("worker");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Workers)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("worker_role_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
