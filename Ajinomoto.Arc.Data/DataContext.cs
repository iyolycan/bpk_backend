using Microsoft.EntityFrameworkCore;
using Ajinomoto.Arc.Data.Models;

namespace Ajinomoto.Arc.Data
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppAction> AppActions { get; set; } = null!;
        public virtual DbSet<AppConfig> AppConfigs { get; set; } = null!;
        public virtual DbSet<AppUser> AppUsers { get; set; } = null!;
        public virtual DbSet<AppUserArea> AppUserAreas { get; set; } = null!;
        public virtual DbSet<AppUserBranch> AppUserBranches { get; set; } = null!;
        public virtual DbSet<Area> Areas { get; set; } = null!;
        public virtual DbSet<Bpk> Bpks { get; set; } = null!;
        public virtual DbSet<BpkDetail> BpkDetails { get; set; } = null!;
        public virtual DbSet<BpkHistory> BpkHistories { get; set; } = null!;
        public virtual DbSet<BpkStatus> BpkStatuses { get; set; } = null!;
        // public virtual DbSet<BpkMasterStatus> BpkMasterStatuses { get; set;} = null!;
        public virtual DbSet<Branch> Branches { get; set; } = null!;
        public virtual DbSet<ClearingStatus> ClearingStatuses { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<DataLevel> DataLevels { get; set; } = null!;
        public virtual DbSet<IncomingPayment> IncomingPayments { get; set; } = null!;
        public virtual DbSet<InvoiceDetails> InvoiceDetails { get; set; } = null!;
        public virtual DbSet<InvoiceInbox> InvoiceInbox { get; set; } = null!;
        public virtual DbSet<IncomingPaymentCutOff> IncomingPaymentCutOffs { get; set; } = null!;
        public virtual DbSet<IncomingPaymentNonSpm> IncomingPaymentNonSpms { get; set; } = null!;
        public virtual DbSet<IncomingPaymentView> IncomingPaymentViews { get; set; } = null!;
        public virtual DbSet<IncomingSpm> IncomingSpms { get; set; } = null!;
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<KpiProperty> KpiProperties { get; set; } = null!;
        public virtual DbSet<KpiSummary> KpiSummaries { get; set; } = null!;
        public virtual DbSet<Potongan> Potongans { get; set; } = null!;
        public virtual DbSet<PotonganType> PotonganTypes { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<RoleArea> RoleAreas { get; set; } = null!;
        public virtual DbSet<RoleBranch> RoleBranches { get; set; } = null!;
        public virtual DbSet<Segment> Segments { get; set; } = null!;
        public virtual DbSet<SegmentConfig> SegmentConfigs { get; set; } = null!;
        public virtual DbSet<SegmentKpiProperty> SegmentKpiProperties { get; set; } = null!;
        public virtual DbSet<Source> Sources { get; set; } = null!;
        public virtual DbSet<TemplateUploadType> TemplateUploadTypes { get; set; } = null!;
        public virtual DbSet<UserView> UserViews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<AppAction>(entity =>
            {
                entity.Property(e => e.AppActionId).ValueGeneratedNever();
            });

            modelBuilder.Entity<AppConfig>(entity =>
            {
                entity.Property(e => e.AppConfigId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.Revision).HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.AppUserId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.Password)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AppUsers)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("app_user_ibfk_2");
            });

            modelBuilder.Entity<AppUserArea>(entity =>
            {
                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.AppUserAreas)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("app_user_area_ibfk_3");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.AppUserAreas)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("app_user_area_ibfk_2");
            });

            modelBuilder.Entity<AppUserBranch>(entity =>
            {
                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.AppUserBranches)
                    .HasForeignKey(d => d.AppUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__app_user");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.AppUserBranches)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__branch");
            });

            modelBuilder.Entity<Area>(entity =>
            {
                entity.Property(e => e.AreaId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Areas)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("area_ibfk_1");
            });

            modelBuilder.Entity<Bpk>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.HasOne(d => d.BpkStatus)
                    .WithMany(p => p.Bpks)
                    .HasForeignKey(d => d.BpkStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bpk_ibfk_4");
            });

            modelBuilder.Entity<BpkDetail>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.HasOne(d => d.Bpk)
                    .WithMany(p => p.BpkDetails)
                    .HasForeignKey(d => d.BpkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bpk_detail_ibfk_1");

                entity.HasOne(d => d.Potongan)
                    .WithOne(p => p.BpkDetail)
                    .HasForeignKey<BpkDetail>(d => d.PotonganId)
                    .HasConstraintName("bpk_detail_ibfk_2");
            });

            modelBuilder.Entity<BpkHistory>(entity =>
            {
                entity.Property(e => e.ActionAt).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.HasOne(d => d.AppAction)
                    .WithMany(p => p.BpkHistories)
                    .HasForeignKey(d => d.AppActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bpk_history_app_action");

                entity.HasOne(d => d.Bpk)
                    .WithMany(p => p.BpkHistories)
                    .HasForeignKey(d => d.BpkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bpk_history_ibfk_1");

                entity.HasOne(d => d.BpkStatus)
                    .WithMany(p => p.BpkHistories)
                    .HasForeignKey(d => d.BpkStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bpk_history_ibfk_2");

                entity.HasOne(d => d.ClearingStatus)
                    .WithMany(p => p.BpkHistories)
                    .HasForeignKey(d => d.ClearingStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bpk_history_clearing_status");
            });

            modelBuilder.Entity<BpkStatus>(entity =>
            {
                entity.Property(e => e.BpkStatusId).ValueGeneratedNever();
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.Property(e => e.BranchId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");
            });

            modelBuilder.Entity<ClearingStatus>(entity =>
            {
                entity.Property(e => e.ClearingStatusId).ValueGeneratedNever();
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");
            });

            modelBuilder.Entity<DataLevel>(entity =>
            {
                entity.Property(e => e.DataLevelId).ValueGeneratedNever();
            });

            modelBuilder.Entity<IncomingPayment>(entity =>
            {
                entity.Property(e => e.ClearingStatusId).HasDefaultValueSql("'1'");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.IncomingPayments)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("incoming_payment_ibfk_4");

                entity.HasOne(d => d.Bpk)
                    .WithMany(p => p.IncomingPayments)
                    .HasForeignKey(d => d.BpkId)
                    .HasConstraintName("incoming_payment_ibfk_6");

                entity.HasOne(d => d.ClearingStatus)
                    .WithMany(p => p.IncomingPayments)
                    .HasForeignKey(d => d.ClearingStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("incoming_payment_ibfk_7");

                entity.HasOne(d => d.Pic)
                    .WithMany(p => p.IncomingPayments)
                    .HasForeignKey(d => d.PicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("incoming_payment_ibfk_8");

                entity.HasOne(d => d.Segment)
                    .WithMany(p => p.IncomingPayments)
                    .HasForeignKey(d => d.SegmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("incoming_payment_ibfk_1");

                entity.HasOne(d => d.Source)
                    .WithMany(p => p.IncomingPayments)
                    .HasForeignKey(d => d.SourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("incoming_payment_ibfk_2");
            });

            modelBuilder.Entity<IncomingPaymentCutOff>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");
            });

            modelBuilder.Entity<IncomingPaymentNonSpm>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.HasOne(d => d.Pic)
                    .WithMany(p => p.IncomingPaymentNonSpms)
                    .HasForeignKey(d => d.PicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("incoming_payment_non_spm_ibfk_3");

                entity.HasOne(d => d.Segment)
                    .WithMany(p => p.IncomingPaymentNonSpms)
                    .HasForeignKey(d => d.SegmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("incoming_payment_non_spm_ibfk_1");

                entity.HasOne(d => d.Source)
                    .WithMany(p => p.IncomingPaymentNonSpms)
                    .HasForeignKey(d => d.SourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("incoming_payment_non_spm_ibfk_2");
            });

            modelBuilder.Entity<IncomingPaymentView>(entity =>
            {
                entity.ToView("incoming_payment_view");
                // entity.ToView("incoming_payment_view_dua");

                entity.Property(e => e.ClearingStatusId).HasDefaultValueSql("'1'");

                entity.Property(e => e.No)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
            });

            modelBuilder.Entity<IncomingSpm>(entity =>
            {
                entity.ToView("incoming_spm");

                entity.Property(e => e.ClearingStatusId).HasDefaultValueSql("'1'");

                entity.Property(e => e.No)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");
            });

            modelBuilder.Entity<KpiProperty>(entity =>
            {
                entity.Property(e => e.KpiPropertyId).ValueGeneratedNever();

                entity.Property(e => e.Name).HasDefaultValueSql("''");
            });

            modelBuilder.Entity<KpiSummary>(entity =>
            {
                entity.ToView("kpi_summary");
            });

            modelBuilder.Entity<Potongan>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.Potongans)
                    .HasForeignKey(d => d.BranchId)
                    .HasConstraintName("FK_potongan_branch");

                entity.HasOne(d => d.PotonganType)
                    .WithMany(p => p.Potongans)
                    .HasForeignKey(d => d.PotonganTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("potongan_ibfk_1");
            });

            modelBuilder.Entity<PotonganType>(entity =>
            {
                entity.Property(e => e.PotonganTypeId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.DataLevelId).HasDefaultValueSql("'3'");

                entity.HasOne(d => d.DataLevel)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(d => d.DataLevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_role_data_level");
            });

            modelBuilder.Entity<RoleArea>(entity =>
            {
                entity.HasOne(d => d.Area)
                    .WithMany(p => p.RoleAreas)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__area");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleAreas)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__role");
            });

            modelBuilder.Entity<RoleBranch>(entity =>
            {
                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.RoleBranches)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_role_branch_branch");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleBranches)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_role_branch_role");
            });

            modelBuilder.Entity<Segment>(entity =>
            {
                entity.Property(e => e.SegmentId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.Revision).HasDefaultValueSql("'0'");

                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.HasOne(d => d.KpiPropertyCurrent)
                    .WithMany(p => p.SegmentKpiPropertyCurrents)
                    .HasForeignKey(d => d.KpiPropertyCurrentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("segment_ibfk_4");

                entity.HasOne(d => d.KpiPropertyTotal)
                    .WithMany(p => p.SegmentKpiPropertyTotals)
                    .HasForeignKey(d => d.KpiPropertyTotalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("segment_ibfk_5");

                entity.HasOne(d => d.TemplateUploadType)
                    .WithMany(p => p.Segments)
                    .HasForeignKey(d => d.TemplateUploadTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("segment_ibfk_3");
            });

            modelBuilder.Entity<SegmentConfig>(entity =>
            {
                entity.ToView("segment_config");

                entity.Property(e => e.Comparation1).HasDefaultValueSql("''");

                entity.Property(e => e.Comparation2).HasDefaultValueSql("''");

                entity.Property(e => e.Empty).HasDefaultValueSql("''");
            });

            modelBuilder.Entity<SegmentKpiProperty>(entity =>
            {
                entity.HasOne(d => d.KpiProperty)
                    .WithMany()
                    .HasForeignKey(d => d.KpiPropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_segment_kpi_property_kpi_property");

                entity.HasOne(d => d.Segment)
                    .WithMany()
                    .HasForeignKey(d => d.SegmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_segment_kpi_property_segment");
            });

            modelBuilder.Entity<Source>(entity =>
            {
                entity.Property(e => e.SourceId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("'0000-00-00 00:00:00'");
            });

            modelBuilder.Entity<TemplateUploadType>(entity =>
            {
                entity.Property(e => e.TemplateUploadTypeId).ValueGeneratedNever();

                entity.Property(e => e.Name).HasDefaultValueSql("''");

                entity.Property(e => e.TemplateFileName).HasDefaultValueSql("''");
            });

            modelBuilder.Entity<UserView>(entity =>
            {
                entity.ToView("user_view");

                entity.Property(e => e.No)
                    .HasDefaultValueSql("''")
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
