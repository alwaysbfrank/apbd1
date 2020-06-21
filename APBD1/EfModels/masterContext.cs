using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace APBD1.EfModels
{
    public partial class masterContext : DbContext
    {
        public masterContext()
        {
        }

        public masterContext(DbContextOptions<masterContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Auth> Auth { get; set; }
        public virtual DbSet<Enrollments> Enrollments { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Studies> Studies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auth>(entity =>
            {
                entity.HasKey(e => e.IndexNumber)
                    .HasName("PK__Auth__98DAB2EB8489E29C");

                entity.Property(e => e.IndexNumber)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RefreshToken)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Roles)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Salt)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IndexNumberNavigation)
                    .WithOne(p => p.Auth)
                    .HasForeignKey<Auth>(d => d.IndexNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Auth__IndexNumbe__07220AB2");
            });

            modelBuilder.Entity<Enrollments>(entity =>
            {
                entity.HasKey(e => e.IdEnrollment)
                    .HasName("PK__Enrollme__5EBB800FDB984E8C");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.IdStudyNavigation)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.IdStudy)
                    .HasConstraintName("FK__Enrollmen__IdStu__0169315C");
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.HasKey(e => e.IndexNumber)
                    .HasName("PK__Students__98DAB2EB84B023A4");

                entity.Property(e => e.IndexNumber)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdEnrollmentNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.IdEnrollment)
                    .HasConstraintName("FK__Students__IdEnro__04459E07");
            });

            modelBuilder.Entity<Studies>(entity =>
            {
                entity.HasKey(e => e.IdStudy)
                    .HasName("PK__Studies__2B1257D3BB0C333F");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
