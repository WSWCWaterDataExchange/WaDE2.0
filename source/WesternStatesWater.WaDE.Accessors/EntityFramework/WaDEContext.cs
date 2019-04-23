using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class WaDEContext : DbContext
    {
        public WaDEContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; set; }

        public virtual DbSet<AggBridgeBeneficialUsesFact> AggBridgeBeneficialUsesFact { get; set; }
        public virtual DbSet<AggregatedAmountsFact> AggregatedAmountsFact { get; set; }
        public virtual DbSet<AggregationStatistic> AggregationStatistic { get; set; }
        public virtual DbSet<AllocationAmountsFact> AllocationAmountsFact { get; set; }
        public virtual DbSet<AllocationBridgeBeneficialUsesFact> AllocationBridgeBeneficialUsesFact { get; set; }
        public virtual DbSet<AllocationsDim> AllocationsDim { get; set; }
        public virtual DbSet<AllocationsDimInput> AllocationsDimInput { get; set; }
        public virtual DbSet<BeneficialUsesDim> BeneficialUsesDim { get; set; }
        public virtual DbSet<CropType> CropType { get; set; }
        public virtual DbSet<DateDim> DateDim { get; set; }
        public virtual DbSet<Epsgcode> Epsgcode { get; set; }
        public virtual DbSet<GnisfeatureName> GnisfeatureName { get; set; }
        public virtual DbSet<IrrigationMethod> IrrigationMethod { get; set; }
        public virtual DbSet<LegalStatus> LegalStatus { get; set; }
        public virtual DbSet<MethodType> MethodType { get; set; }
        public virtual DbSet<MethodsDim> MethodsDim { get; set; }
        public virtual DbSet<Naicscode> Naicscode { get; set; }
        public virtual DbSet<Nhdmetadata> Nhdmetadata { get; set; }
        public virtual DbSet<NhdnetworkStatus> NhdnetworkStatus { get; set; }
        public virtual DbSet<Nhdproduct> Nhdproduct { get; set; }
        public virtual DbSet<OrganizationsDim> OrganizationsDim { get; set; }
        public virtual DbSet<RegulatoryOverlayDim> RegulatoryOverlayDim { get; set; }
        public virtual DbSet<RegulatoryReportingUnitsFact> RegulatoryReportingUnitsFact { get; set; }
        public virtual DbSet<RegulatoryStatus> RegulatoryStatus { get; set; }
        public virtual DbSet<ReportYearCv> ReportYearCv { get; set; }
        public virtual DbSet<ReportYearType> ReportYearType { get; set; }
        public virtual DbSet<ReportingUnitType> ReportingUnitType { get; set; }
        public virtual DbSet<ReportingUnitsDim> ReportingUnitsDim { get; set; }
        public virtual DbSet<SiteVariableAmountsFact> SiteVariableAmountsFact { get; set; }
        public virtual DbSet<SitesBridgeBeneficialUsesFact> SitesBridgeBeneficialUsesFact { get; set; }
        public virtual DbSet<SitesDim> SitesDim { get; set; }
        public virtual DbSet<Units> Units { get; set; }
        public virtual DbSet<Usgscategory> Usgscategory { get; set; }
        public virtual DbSet<Variable> Variable { get; set; }
        public virtual DbSet<VariableSpecific> VariableSpecific { get; set; }
        public virtual DbSet<VariablesDim> VariablesDim { get; set; }
        public virtual DbSet<WaterAllocationBasis> WaterAllocationBasis { get; set; }
        public virtual DbSet<WaterQualityIndicator> WaterQualityIndicator { get; set; }
        public virtual DbSet<WaterRightType> WaterRightType { get; set; }
        public virtual DbSet<WaterSourceType> WaterSourceType { get; set; }
        public virtual DbSet<WaterSourcesDim> WaterSourcesDim { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("WadeDatabase"), x =>
                {
                    x.UseNetTopologySuite();
                    x.EnableRetryOnFailure();
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<AggBridgeBeneficialUsesFact>(entity =>
            {
                entity.HasKey(e => e.AggBridgeId)
                    .HasName("pkAggBridge_BeneficialUses_fact");

                entity.ToTable("AggBridge_BeneficialUses_fact", "Core");

                entity.Property(e => e.AggBridgeId).HasColumnName("AggBridgeID");

                entity.Property(e => e.AggregatedAmountId).HasColumnName("AggregatedAmountID");

                entity.Property(e => e.BeneficialUseId).HasColumnName("BeneficialUseID");

                entity.HasOne(d => d.AggregatedAmount)
                    .WithMany(p => p.AggBridgeBeneficialUsesFact)
                    .HasForeignKey(d => d.AggregatedAmountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AggBridge_BeneficialUses_fact_AggregatedAmounts_fact");

                entity.HasOne(d => d.BeneficialUse)
                    .WithMany(p => p.AggBridgeBeneficialUsesFact)
                    .HasForeignKey(d => d.BeneficialUseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AggBridge_BeneficialUses_fact_BeneficialUses_dim");
            });

            modelBuilder.Entity<AggregatedAmountsFact>(entity =>
            {
                entity.HasKey(e => e.AggregatedAmountId)
                    .HasName("pkAggregatedAmounts_fact");

                entity.ToTable("AggregatedAmounts_fact", "Core");

                entity.Property(e => e.AggregatedAmountId).HasColumnName("AggregatedAmountID");

                entity.Property(e => e.BeneficialUseId).HasColumnName("BeneficialUseID");

                entity.Property(e => e.InterbasinTransferFromId)
                    .HasColumnName("InterbasinTransferFromID")
                    .HasMaxLength(100);

                entity.Property(e => e.InterbasinTransferToId)
                    .HasColumnName("InterbasinTransferToID")
                    .HasMaxLength(100);

                entity.Property(e => e.MethodId).HasColumnName("MethodID");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.PowerGeneratedGwh).HasColumnName("PowerGeneratedGWh");

                entity.Property(e => e.ReportYearCV).HasMaxLength(4);

                entity.Property(e => e.ReportingUnitId).HasColumnName("ReportingUnitID");

                entity.Property(e => e.TimeframeEndId).HasColumnName("TimeframeEndID");

                entity.Property(e => e.TimeframeStartId).HasColumnName("TimeframeStartID");

                entity.Property(e => e.VariableSpecificId).HasColumnName("VariableSpecificID");

                entity.Property(e => e.WaterSourceId).HasColumnName("WaterSourceID");

                entity.HasOne(d => d.BeneficialUse)
                    .WithMany(p => p.AggregatedAmountsFact)
                    .HasForeignKey(d => d.BeneficialUseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AggregatedAmounts_fact_BeneficialUses_dim");

                entity.HasOne(d => d.DataPublicationDateNavigation)
                    .WithMany(p => p.AggregatedAmountsFactDataPublicationDateNavigation)
                    .HasForeignKey(d => d.DataPublicationDate)
                    .HasConstraintName("fk_AggregatedAmounts_Date_dim_end_pub");

                entity.HasOne(d => d.Method)
                    .WithMany(p => p.AggregatedAmountsFact)
                    .HasForeignKey(d => d.MethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AggregatedAmounts_fact_Methods_dim");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.AggregatedAmountsFact)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AggregatedAmounts_fact_Organizations_dim");

                entity.HasOne(d => d.ReportYearNavigation)
                    .WithMany(p => p.AggregatedAmountsFact)
                    .HasForeignKey(d => d.ReportYearCV)
                    .HasConstraintName("fk_AggregatedAmounts_fact_ReportYearCV");

                entity.HasOne(d => d.ReportingUnit)
                    .WithMany(p => p.AggregatedAmountsFact)
                    .HasForeignKey(d => d.ReportingUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AggregatedAmounts_fact_ReportingUnits_dim");

                entity.HasOne(d => d.TimeframeEnd)
                    .WithMany(p => p.AggregatedAmountsFactTimeframeEnd)
                    .HasForeignKey(d => d.TimeframeEndId)
                    .HasConstraintName("fk_AggregatedAmounts_Date_dim_end");

                entity.HasOne(d => d.TimeframeStart)
                    .WithMany(p => p.AggregatedAmountsFactTimeframeStart)
                    .HasForeignKey(d => d.TimeframeStartId)
                    .HasConstraintName("fk_AggregatedAmounts_Date_dim_start");

                entity.HasOne(d => d.VariableSpecific)
                    .WithMany(p => p.AggregatedAmountsFact)
                    .HasForeignKey(d => d.VariableSpecificId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AggregatedAmounts_fact_Variables_dim");

                entity.HasOne(d => d.WaterSource)
                    .WithMany(p => p.AggregatedAmountsFact)
                    .HasForeignKey(d => d.WaterSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AggregatedAmounts_fact_WaterSources_dim");
            });

            modelBuilder.Entity<AggregationStatistic>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_AggregationStatistic");

                entity.ToTable("AggregationStatistic", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AllocationAmountsFact>(entity =>
            {
                entity.HasKey(e => e.AllocationAmountId)
                    .HasName("pkAllocationAmounts_fact");

                entity.ToTable("AllocationAmounts_fact", "Core");

                entity.Property(e => e.AllocationAmountId).HasColumnName("AllocationAmountID");

                entity.Property(e => e.AllocationCommunityWaterSupplySystem).HasMaxLength(250);

                entity.Property(e => e.AllocationId).HasColumnName("AllocationID");

                entity.Property(e => e.DataPublicationDateId).HasColumnName("DataPublicationDateID");

                entity.Property(e => e.Geometry).HasColumnType("geometry");

                entity.Property(e => e.MethodId).HasColumnName("MethodID");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.PowerGeneratedGwh).HasColumnName("PowerGeneratedGWh");

                entity.Property(e => e.PrimaryBeneficialUseId).HasColumnName("PrimaryBeneficialUseID");

                entity.Property(e => e.ReportYear)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(e => e.Sdwisidentifier)
                    .HasColumnName("SDWISIdentifier")
                    .HasMaxLength(250);

                entity.Property(e => e.SiteId).HasColumnName("SiteID");

                entity.Property(e => e.TimeframeEndDateId).HasColumnName("TimeframeEndDateID");

                entity.Property(e => e.TimeframeStartDateId).HasColumnName("TimeframeStartDateID");

                entity.Property(e => e.VariableSpecificId).HasColumnName("VariableSpecificID");

                entity.Property(e => e.WaterSourceId).HasColumnName("WaterSourceID");

                entity.HasOne(d => d.Allocation)
                    .WithMany(p => p.AllocationAmountsFact)
                    .HasForeignKey(d => d.AllocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AllocationAmounts_fact_Allocations_dim");

                entity.HasOne(d => d.DataPublicationDate)
                    .WithMany(p => p.AllocationAmountsFactDataPublicationDate)
                    .HasForeignKey(d => d.DataPublicationDateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AllocationAmounts_fact_Date_pub");

                entity.HasOne(d => d.Method)
                    .WithMany(p => p.AllocationAmountsFact)
                    .HasForeignKey(d => d.MethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AllocationAmounts_fact_Methods_dim");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.AllocationAmountsFact)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AllocationAmounts_fact_Organizations_dim");

                entity.HasOne(d => d.PrimaryBeneficialUse)
                    .WithMany(p => p.AllocationAmountsFact)
                    .HasForeignKey(d => d.PrimaryBeneficialUseId)
                    .HasConstraintName("fk_AllocationAmounts_fact_BeneficialUses_dim");

                entity.HasOne(d => d.ReportYearNavigation)
                    .WithMany(p => p.AllocationAmountsFact)
                    .HasForeignKey(d => d.ReportYear)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AllocationAmounts_fact_ReportYearCV");

                entity.HasOne(d => d.Site)
                    .WithMany(p => p.AllocationAmountsFact)
                    .HasForeignKey(d => d.SiteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AllocationAmounts_fact_Sites_dim");

                entity.HasOne(d => d.TimeframeEndDate)
                    .WithMany(p => p.AllocationAmountsFactTimeframeEndDate)
                    .HasForeignKey(d => d.TimeframeEndDateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AllocationAmounts_fact_Date_dim_end");

                entity.HasOne(d => d.TimeframeStartDate)
                    .WithMany(p => p.AllocationAmountsFactTimeframeStartDate)
                    .HasForeignKey(d => d.TimeframeStartDateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AllocationAmounts_fact_Date_dim_start");

                entity.HasOne(d => d.VariableSpecific)
                    .WithMany(p => p.AllocationAmountsFact)
                    .HasForeignKey(d => d.VariableSpecificId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AllocationAmounts_fact_Variables_dim");

                entity.HasOne(d => d.WaterSource)
                    .WithMany(p => p.AllocationAmountsFact)
                    .HasForeignKey(d => d.WaterSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AllocationAmounts_fact_WaterSources_dim");
            });

            modelBuilder.Entity<AllocationBridgeBeneficialUsesFact>(entity =>
            {
                entity.HasKey(e => e.AllocationBridgeId)
                    .HasName("pkAllocationBridge_BeneficialUses_fact");

                entity.ToTable("AllocationBridge_BeneficialUses_fact", "Core");

                entity.Property(e => e.AllocationBridgeId).HasColumnName("AllocationBridgeID");

                entity.Property(e => e.AllocationAmountId).HasColumnName("AllocationAmountID");

                entity.Property(e => e.BeneficialUseId).HasColumnName("BeneficialUseID");

                entity.HasOne(d => d.AllocationAmount)
                    .WithMany(p => p.AllocationBridgeBeneficialUsesFact)
                    .HasForeignKey(d => d.AllocationAmountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AllocationBridge_BeneficialUses_fact_AllocationAmounts_fact");

                entity.HasOne(d => d.BeneficialUse)
                    .WithMany(p => p.AllocationBridgeBeneficialUsesFact)
                    .HasForeignKey(d => d.BeneficialUseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_AllocationBridge_BeneficialUses_fact_BeneficialUses_dim");
            });

            modelBuilder.Entity<AllocationsDim>(entity =>
            {
                entity.HasKey(e => e.AllocationId)
                    .HasName("pkAllocations_dim");

                entity.ToTable("Allocations_dim", "Core");

                entity.Property(e => e.AllocationId).HasColumnName("AllocationID");

                entity.Property(e => e.AllocationBasisCv)
                    .HasColumnName("AllocationBasisCV")
                    .HasMaxLength(250);

                entity.Property(e => e.AllocationChangeApplicationIndicator).HasMaxLength(100);

                entity.Property(e => e.AllocationLegalStatusCv)
                    .IsRequired()
                    .HasColumnName("AllocationLegalStatusCV")
                    .HasMaxLength(50);

                entity.Property(e => e.AllocationNativeId)
                    .IsRequired()
                    .HasColumnName("AllocationNativeID")
                    .HasMaxLength(250);

                entity.Property(e => e.AllocationOwner)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.AllocationUuid)
                    .IsRequired()
                    .HasColumnName("AllocationUUID")
                    .HasMaxLength(50);

                entity.Property(e => e.LegacyAllocationIds)
                    .HasColumnName("LegacyAllocationIDs")
                    .HasMaxLength(100);

                entity.Property(e => e.WaterRightTypeCv)
                    .HasColumnName("WaterRightTypeCV")
                    .HasMaxLength(10);

                entity.HasOne(d => d.AllocationApplicationDateNavigation)
                    .WithMany(p => p.AllocationsDimAllocationApplicationDateNavigation)
                    .HasForeignKey(d => d.AllocationApplicationDate)
                    .HasConstraintName("fk_Allocations_dim_Date_dim_app");

                entity.HasOne(d => d.AllocationExpirationDateNavigation)
                    .WithMany(p => p.AllocationsDimAllocationExpirationDateNavigation)
                    .HasForeignKey(d => d.AllocationExpirationDate)
                    .HasConstraintName("fk_Allocations_dim_Date_dim_exp");

                entity.HasOne(d => d.AllocationPriorityDateNavigation)
                    .WithMany(p => p.AllocationsDimAllocationPriorityDateNavigation)
                    .HasForeignKey(d => d.AllocationPriorityDate)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Allocations_dim_Date_dim_prio");
            });

            modelBuilder.Entity<AllocationsDimInput>(entity =>
            {
                entity.HasKey(e => e.AllocationNativeId)
                    .HasName("pkAllocations_dim_Input");

                entity.ToTable("Allocations_dim_Input", "Core");

                entity.Property(e => e.AllocationNativeId)
                    .HasColumnName("AllocationNativeID")
                    .HasMaxLength(250)
                    .ValueGeneratedNever();

                entity.Property(e => e.AllocationBasisCv)
                    .HasColumnName("AllocationBasisCV")
                    .HasMaxLength(250);

                entity.Property(e => e.AllocationChangeApplicationIndicator).HasMaxLength(100);

                entity.Property(e => e.AllocationLegalStatusCv)
                    .IsRequired()
                    .HasColumnName("AllocationLegalStatusCV")
                    .HasMaxLength(50);

                entity.Property(e => e.AllocationOwner)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.LegacyAllocationIds)
                    .HasColumnName("LegacyAllocationIDs")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<BeneficialUsesDim>(entity =>
            {
                entity.HasKey(e => e.BeneficialUseId)
                    .HasName("pkBeneficialUses_dim");

                entity.ToTable("BeneficialUses_dim", "Core");

                entity.Property(e => e.BeneficialUseId).HasColumnName("BeneficialUseID");

                entity.Property(e => e.BeneficialUseCategory)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.BeneficialUseUuid)
                    .HasColumnName("BeneficialUseUUID")
                    .HasMaxLength(500);

                entity.Property(e => e.NaicscodeNameCv)
                    .HasColumnName("NAICSCodeNameCV")
                    .HasMaxLength(250);

                entity.Property(e => e.PrimaryUseCategory).HasMaxLength(250);

                entity.Property(e => e.UsgscategoryNameCv)
                    .HasColumnName("USGSCategoryNameCV")
                    .HasMaxLength(250);

                entity.HasOne(d => d.NaicscodeNameCvNavigation)
                    .WithMany(p => p.BeneficialUsesDim)
                    .HasForeignKey(d => d.NaicscodeNameCv)
                    .HasConstraintName("fk_BeneficialUses_dim_NAICSCode");

                entity.HasOne(d => d.UsgscategoryNameCvNavigation)
                    .WithMany(p => p.BeneficialUsesDim)
                    .HasForeignKey(d => d.UsgscategoryNameCv)
                    .HasConstraintName("fk_BeneficialUses_dim_USGSCategory");
            });

            modelBuilder.Entity<CropType>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_CropType");

                entity.ToTable("CropType", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DateDim>(entity =>
            {
                entity.HasKey(e => e.DateId)
                    .HasName("pkDate_dim");

                entity.ToTable("Date_dim", "Core");

                entity.Property(e => e.DateId).HasColumnName("DateID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Year).HasMaxLength(4);
            });

            modelBuilder.Entity<Epsgcode>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_EPSGCode");

                entity.ToTable("EPSGCode", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<GnisfeatureName>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_GNISFeatureName");

                entity.ToTable("GNISFeatureName", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<IrrigationMethod>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_IrrigationMethod");

                entity.ToTable("IrrigationMethod", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LegalStatus>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_LegalStatus");

                entity.ToTable("LegalStatus", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.StateIdentifier).HasMaxLength(2);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MethodType>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_VariableType_dup");

                entity.ToTable("MethodType", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MethodsDim>(entity =>
            {
                entity.HasKey(e => e.MethodId)
                    .HasName("pkMethods_dim");

                entity.ToTable("Methods_dim", "Core");

                entity.Property(e => e.MethodId).HasColumnName("MethodID");

                entity.Property(e => e.ApplicableResourceTypeCv)
                    .IsRequired()
                    .HasColumnName("ApplicableResourceTypeCV")
                    .HasMaxLength(100);

                entity.Property(e => e.DataConfidenceValue).HasMaxLength(50);

                entity.Property(e => e.DataCoverageValue).HasMaxLength(100);

                entity.Property(e => e.DataQualityValueCv)
                    .HasColumnName("DataQualityValueCV")
                    .HasMaxLength(50);

                entity.Property(e => e.MethodDescription)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.MethodName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MethodNemilink)
                    .HasColumnName("MethodNEMILink")
                    .HasMaxLength(100);

                entity.Property(e => e.MethodTypeCv)
                    .IsRequired()
                    .HasColumnName("MethodTypeCV")
                    .HasMaxLength(50);

                entity.Property(e => e.MethodUuid)
                    .IsRequired()
                    .HasColumnName("MethodUUID")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Naicscode>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_VariableType_dup_dup_dup_dup_dup_dup");

                entity.ToTable("NAICSCode", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category).HasMaxLength(250);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<Nhdmetadata>(entity =>
            {
                entity.ToTable("NHDMetadata", "Core");

                entity.Property(e => e.NhdmetadataId).HasColumnName("NHDMetadataID");

                entity.Property(e => e.NhdmeasureNumber)
                    .HasColumnName("NHDMeasureNumber")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NhdnetworkStatusCv)
                    .IsRequired()
                    .HasColumnName("NHDNetworkStatusCV")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NhdproductCv)
                    .HasColumnName("NHDProductCV")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NhdreachCode)
                    .HasColumnName("NHDReachCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NhdupdateDate)
                    .HasColumnName("NHDUpdateDate")
                    .HasColumnType("date");
            });

            modelBuilder.Entity<NhdnetworkStatus>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_NHDNetworkStatus");

                entity.ToTable("NHDNetworkStatus", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Nhdproduct>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_NHDProduct");

                entity.ToTable("NHDProduct", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OrganizationsDim>(entity =>
            {
                entity.HasKey(e => e.OrganizationId)
                    .HasName("pkOrganizations_dim");

                entity.ToTable("Organizations_dim", "Core");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.OrganizationContactEmail)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.OrganizationContactName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.OrganizationName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.OrganizationPhoneNumber)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.OrganizationPurview).HasMaxLength(250);

                entity.Property(e => e.OrganizationUuid)
                    .IsRequired()
                    .HasColumnName("OrganizationUUID")
                    .HasMaxLength(250);

                entity.Property(e => e.OrganizationWebsite)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<RegulatoryOverlayDim>(entity =>
            {
                entity.HasKey(e => e.RegulatoryOverlayId)
                    .HasName("pkRegulatoryOverlay_dim");

                entity.ToTable("RegulatoryOverlay_dim", "Core");

                entity.Property(e => e.RegulatoryOverlayId).HasColumnName("RegulatoryOverlayID");

                entity.Property(e => e.OversightAgency)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.RegulatoryDescription).IsRequired();

                entity.Property(e => e.RegulatoryName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.RegulatoryOverlayNativeId)
                    .HasColumnName("RegulatoryOverlayNativeID")
                    .HasMaxLength(250);

                entity.Property(e => e.RegulatoryOverlayUuid)
                    .HasColumnName("RegulatoryOverlayUUID")
                    .HasMaxLength(250);

                entity.Property(e => e.RegulatoryStatusCv)
                    .IsRequired()
                    .HasColumnName("RegulatoryStatusCV")
                    .HasMaxLength(50);

                entity.Property(e => e.RegulatoryStatute).HasMaxLength(500);

                entity.Property(e => e.RegulatoryStatuteLink)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                //entity.Property(e => e.ReportYearStartMonth)
                //    .IsRequired()
                //    .HasMaxLength(5);

                //entity.Property(e => e.ReportYearTypeCv)
                //    .IsRequired()
                //    .HasColumnName("ReportYearTypeCV")
                //    .HasMaxLength(10);

                //entity.Property(e => e.TimeframeEndId).HasColumnName("TimeframeEndID");

                //entity.Property(e => e.TimeframeStartId).HasColumnName("TimeframeStartID");

                //entity.HasOne(d => d.TimeframeEnd)
                //    .WithMany(p => p.RegulatoryOverlayDimTimeframeEnd)
                //    .HasForeignKey(d => d.TimeframeEndId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("fk_RegulatoryOverlay_dim_Date_dim_end");

                //entity.HasOne(d => d.TimeframeStart)
                //    .WithMany(p => p.RegulatoryOverlayDimTimeframeStart)
                //    .HasForeignKey(d => d.TimeframeStartId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("fk_RegulatoryOverlay_dim_Date_dim_start");
            });

            modelBuilder.Entity<RegulatoryReportingUnitsFact>(entity =>
            {
                entity.HasKey(e => e.BridgeId)
                    .HasName("pkRegulatoryReportingUnits_fact");

                entity.ToTable("RegulatoryReportingUnits_fact", "Core");

                entity.Property(e => e.BridgeId)
                    .HasColumnName("BridgeID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DataPublicationDateId).HasColumnName("DataPublicationDateID");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.RegulatoryOverlayId).HasColumnName("RegulatoryOverlayID");

                entity.Property(e => e.ReportYearCv)
                    .IsRequired()
                    .HasColumnName("ReportYearCV")
                    .HasMaxLength(4);

                entity.Property(e => e.ReportingUnitId).HasColumnName("ReportingUnitID");

                entity.HasOne(d => d.DataPublicationDate)
                    .WithMany(p => p.RegulatoryReportingUnitsFact)
                    .HasForeignKey(d => d.DataPublicationDateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_RegulatoryReportingUnits_fact_Date_dim");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.RegulatoryReportingUnitsFact)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_RegulatoryReportingUnits_fact_Organizations_dim");

                entity.HasOne(d => d.RegulatoryOverlay)
                    .WithMany(p => p.RegulatoryReportingUnitsFact)
                    .HasForeignKey(d => d.RegulatoryOverlayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_RegulatoryReportingUnits_fact_RegulatoryOverlay_dim");

                entity.HasOne(d => d.ReportingUnit)
                    .WithMany(p => p.RegulatoryReportingUnitsFact)
                    .HasForeignKey(d => d.ReportingUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_RegulatoryReportingUnits_fact_ReportingUnits_dim");
            });

            modelBuilder.Entity<RegulatoryStatus>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_RegulatoryStatus");

                entity.ToTable("RegulatoryStatus", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.State).HasMaxLength(2);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReportYearCv>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_ReportYearCV");

                entity.ToTable("ReportYearCV", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(4)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category).HasMaxLength(250);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<ReportYearType>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_ReportYearType");

                entity.ToTable("ReportYearType", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReportingUnitType>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_ReportingUnitType");

                entity.ToTable("ReportingUnitType", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ReportingUnitsDim>(entity =>
            {
                entity.HasKey(e => e.ReportingUnitId)
                    .HasName("pkReportingUnits_dim");

                entity.ToTable("ReportingUnits_dim", "Core");

                entity.Property(e => e.ReportingUnitId).HasColumnName("ReportingUnitID");

                entity.Property(e => e.EpsgcodeCv)
                    .HasColumnName("EPSGCodeCV")
                    .HasMaxLength(50);

                entity.Property(e => e.Geometry).HasColumnType("geometry");

                entity.Property(e => e.ReportingUnitName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.ReportingUnitNativeId)
                    .IsRequired()
                    .HasColumnName("ReportingUnitNativeID")
                    .HasMaxLength(250);

                entity.Property(e => e.ReportingUnitProductVersion).HasMaxLength(100);

                entity.Property(e => e.ReportingUnitTypeCv)
                    .IsRequired()
                    .HasColumnName("ReportingUnitTypeCV")
                    .HasMaxLength(20);

                entity.Property(e => e.ReportingUnitUpdateDate).HasColumnType("date");

                entity.Property(e => e.ReportingUnitUuid)
                    .IsRequired()
                    .HasColumnName("ReportingUnitUUID")
                    .HasMaxLength(250);

                entity.Property(e => e.StateCv)
                    .IsRequired()
                    .HasColumnName("StateCV")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SiteVariableAmountsFact>(entity =>
            {
                entity.HasKey(e => e.SiteVariableAmountId)
                    .HasName("pkSiteVariableAmounts_fact");

                entity.ToTable("SiteVariableAmounts_fact", "Core");

                entity.Property(e => e.SiteVariableAmountId).HasColumnName("SiteVariableAmountID");

                entity.Property(e => e.AllocationId).HasColumnName("AllocationID");

                entity.Property(e => e.BeneficialUseId).HasColumnName("BeneficialUseID");

                entity.Property(e => e.CropTypeCv)
                    .HasColumnName("CropTypeCV")
                    .HasMaxLength(100);

                entity.Property(e => e.Geometry).HasColumnType("geometry");

                entity.Property(e => e.InterbasinTransferFromId)
                    .HasColumnName("InterbasinTransferFromID")
                    .HasMaxLength(100);

                entity.Property(e => e.InterbasinTransferToId)
                    .HasColumnName("InterbasinTransferToID")
                    .HasMaxLength(100);

                entity.Property(e => e.IrrigationMethodCv)
                    .HasColumnName("IrrigationMethodCV")
                    .HasMaxLength(100);

                entity.Property(e => e.MethodId).HasColumnName("MethodID");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.PowerGeneratedGwh).HasColumnName("PowerGeneratedGWh");

                entity.Property(e => e.ReportYear).HasMaxLength(4);

                entity.Property(e => e.SiteId).HasColumnName("SiteID");

                entity.Property(e => e.VariableSpecificId).HasColumnName("VariableSpecificID");

                entity.Property(e => e.WaterSourceId).HasColumnName("WaterSourceID");

                entity.HasOne(d => d.Allocation)
                    .WithMany(p => p.SiteVariableAmountsFact)
                    .HasForeignKey(d => d.AllocationId)
                    .HasConstraintName("fk_SiteVariableAmounts_fact_Allocations_dim");

                entity.HasOne(d => d.BeneficialUse)
                    .WithMany(p => p.SiteVariableAmountsFact)
                    .HasForeignKey(d => d.BeneficialUseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_SiteVariableAmounts_fact_BeneficialUses_dim");

                entity.HasOne(d => d.DataPublicationDateNavigation)
                    .WithMany(p => p.SiteVariableAmountsFactDataPublicationDateNavigation)
                    .HasForeignKey(d => d.DataPublicationDate)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_SiteVariableAmounts_Date_dim_pub");

                entity.HasOne(d => d.Method)
                    .WithMany(p => p.SiteVariableAmountsFact)
                    .HasForeignKey(d => d.MethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_SiteVariableAmounts_fact_Methods_dim");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.SiteVariableAmountsFact)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_SiteVariableAmounts_fact_Organizations_dim");

                entity.HasOne(d => d.ReportYearNavigation)
                    .WithMany(p => p.SiteVariableAmountsFact)
                    .HasForeignKey(d => d.ReportYear)
                    .HasConstraintName("fk_SiteVariableAmounts_fact_ReportYearCV");

                entity.HasOne(d => d.Site)
                    .WithMany(p => p.SiteVariableAmountsFact)
                    .HasForeignKey(d => d.SiteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_SiteVariableAmounts_fact_Sites_dim");

                entity.HasOne(d => d.TimeframeEndNavigation)
                    .WithMany(p => p.SiteVariableAmountsFactTimeframeEndNavigation)
                    .HasForeignKey(d => d.TimeframeEnd)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_SiteVariableAmounts_Date_dim_end");

                entity.HasOne(d => d.TimeframeStartNavigation)
                    .WithMany(p => p.SiteVariableAmountsFactTimeframeStartNavigation)
                    .HasForeignKey(d => d.TimeframeStart)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_SiteVariableAmounts_Date_dim_start");

                entity.HasOne(d => d.VariableSpecific)
                    .WithMany(p => p.SiteVariableAmountsFact)
                    .HasForeignKey(d => d.VariableSpecificId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_SiteVariableAmounts_fact_Variables_dim");

                entity.HasOne(d => d.WaterSource)
                    .WithMany(p => p.SiteVariableAmountsFact)
                    .HasForeignKey(d => d.WaterSourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_SiteVariableAmounts_fact_WaterSources_dim");
            });

            modelBuilder.Entity<SitesBridgeBeneficialUsesFact>(entity =>
            {
                entity.HasKey(e => e.SiteBridgeId)
                    .HasName("pkSitesBridge_BeneficialUses_fact");

                entity.ToTable("SitesBridge_BeneficialUses_fact", "Core");

                entity.Property(e => e.SiteBridgeId)
                    .HasColumnName("SiteBridgeID")
                    .ValueGeneratedNever();

                entity.Property(e => e.BeneficialUseId).HasColumnName("BeneficialUseID");

                entity.Property(e => e.SiteVariableAmountId).HasColumnName("SiteVariableAmountID");

                entity.HasOne(d => d.BeneficialUse)
                    .WithMany(p => p.SitesBridgeBeneficialUsesFact)
                    .HasForeignKey(d => d.BeneficialUseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_SitesBridge_BeneficialUses_fact_BeneficialUses_dim");

                entity.HasOne(d => d.SiteVariableAmount)
                    .WithMany(p => p.SitesBridgeBeneficialUsesFact)
                    .HasForeignKey(d => d.SiteVariableAmountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_SitesBridge_BeneficialUses_fact_SiteVariableAmounts_fact");
            });

            modelBuilder.Entity<SitesDim>(entity =>
            {
                entity.HasKey(e => e.SiteId)
                    .HasName("pkSites_dim");

                entity.ToTable("Sites_dim", "Core");

                entity.Property(e => e.SiteId).HasColumnName("SiteID");

                entity.Property(e => e.CoordinateAccuracy).HasMaxLength(255);

                entity.Property(e => e.CoordinateMethodCv)
                    .IsRequired()
                    .HasColumnName("CoordinateMethodCV")
                    .HasMaxLength(100);

                entity.Property(e => e.Geometry).HasColumnType("geometry");

                entity.Property(e => e.GniscodeCv)
                    .HasColumnName("GNISCodeCV")
                    .HasMaxLength(50);

                entity.Property(e => e.Latitude)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Longitude)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NhdmetadataId).HasColumnName("NHDMetadataID");

                entity.Property(e => e.SiteName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.SiteNativeId)
                    .HasColumnName("SiteNativeID")
                    .HasMaxLength(50);

                entity.Property(e => e.SitePoint).HasColumnType("geometry");

                entity.Property(e => e.SiteTypeCv)
                    .HasColumnName("SiteTypeCV")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SiteUuid)
                    .IsRequired()
                    .HasColumnName("SiteUUID")
                    .HasMaxLength(55);

                entity.HasOne(d => d.Nhdmetadata)
                    .WithMany(p => p.SitesDim)
                    .HasForeignKey(d => d.NhdmetadataId)
                    .HasConstraintName("fk_Sites_NHDMetadata");
            });

            modelBuilder.Entity<Units>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_Units");

                entity.ToTable("Units", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Usgscategory>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_ReportingUnitType_dup");

                entity.ToTable("USGSCategory", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category).HasMaxLength(250);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<Variable>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_Variable");

                entity.ToTable("Variable", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VariableSpecific>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_VariableSpecific");

                entity.ToTable("VariableSpecific", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VariablesDim>(entity =>
            {
                entity.HasKey(e => e.VariableSpecificId)
                    .HasName("pkVariables_dim");

                entity.ToTable("Variables_dim", "Core");

                entity.Property(e => e.VariableSpecificId).HasColumnName("VariableSpecificID");

                entity.Property(e => e.AggregationInterval)
                    .HasColumnName("AggregationInterval ")
                    .HasColumnType("numeric(10, 1)");

                entity.Property(e => e.AggregationIntervalUnitCv)
                    .IsRequired()
                    .HasColumnName("AggregationIntervalUnitCV ")
                    .HasMaxLength(50);

                entity.Property(e => e.AggregationStatisticCv)
                    .IsRequired()
                    .HasColumnName("AggregationStatisticCV")
                    .HasMaxLength(50);

                entity.Property(e => e.AmountUnitCv)
                    .IsRequired()
                    .HasColumnName("AmountUnitCV")
                    .HasMaxLength(250);

                entity.Property(e => e.MaximumAmountUnitCv)
                    .HasColumnName("MaximumAmountUnitCV")
                    .HasMaxLength(255);

                entity.Property(e => e.ReportYearStartMonth)
                    .IsRequired()
                    .HasColumnName("ReportYearStartMonth ")
                    .HasMaxLength(10);

                entity.Property(e => e.ReportYearTypeCv)
                    .IsRequired()
                    .HasColumnName("ReportYearTypeCV ")
                    .HasMaxLength(10);

                entity.Property(e => e.VariableCv)
                    .IsRequired()
                    .HasColumnName("VariableCV")
                    .HasMaxLength(250);

                entity.Property(e => e.VariableSpecificCv)
                    .IsRequired()
                    .HasColumnName("VariableSpecificCV")
                    .HasMaxLength(250);

                entity.Property(e => e.VariableSpecificUuid)
                    .HasColumnName("VariableSpecificUUID")
                    .HasMaxLength(250);

                entity.HasOne(d => d.VariableSpecific)
                    .WithMany(p => p.VariablesDims)
                    .HasForeignKey(d => d.VariableSpecificCv)
                    .HasConstraintName("fk_Variables_dim_VariableSpecific");
            });

            modelBuilder.Entity<WaterAllocationBasis>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_WaterAllocationBasis");

                entity.ToTable("WaterAllocationBasis", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.State).HasMaxLength(2);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WaterQualityIndicator>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_WaterQualityIndicator");

                entity.ToTable("WaterQualityIndicator", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WaterRightType>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkWaterRightType");

                entity.ToTable("WaterRightType", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category).HasMaxLength(250);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250);

                entity.Property(e => e.State).HasMaxLength(2);

                entity.Property(e => e.Term).HasMaxLength(250);
            });

            modelBuilder.Entity<WaterSourceType>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("pkCVs_WaterSourceType");

                entity.ToTable("WaterSourceType", "CVs");

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Definition)
                    .HasMaxLength(5000)
                    .IsUnicode(false);

                entity.Property(e => e.SourceVocabularyUri)
                    .HasColumnName("SourceVocabularyURI")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Term)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WaterSourcesDim>(entity =>
            {
                entity.HasKey(e => e.WaterSourceId)
                    .HasName("pkWaterSources_dim");

                entity.ToTable("WaterSources_dim", "Core");

                entity.Property(e => e.WaterSourceId).HasColumnName("WaterSourceID");

                entity.Property(e => e.Geometry).HasColumnType("geometry");

                entity.Property(e => e.GnisfeatureNameCv)
                    .HasColumnName("GNISFeatureNameCV")
                    .HasMaxLength(250);

                entity.Property(e => e.WaterQualityIndicatorCv)
                    .IsRequired()
                    .HasColumnName("WaterQualityIndicatorCV")
                    .HasMaxLength(100);

                entity.Property(e => e.WaterSourceName).HasMaxLength(250);

                entity.Property(e => e.WaterSourceNativeId)
                    .HasColumnName("WaterSourceNativeID")
                    .HasMaxLength(250);

                entity.Property(e => e.WaterSourceTypeCv)
                    .IsRequired()
                    .HasColumnName("WaterSourceTypeCV")
                    .HasMaxLength(100);

                entity.Property(e => e.WaterSourceUuid)
                    .IsRequired()
                    .HasColumnName("WaterSourceUUID")
                    .HasMaxLength(100);
            });
        }
    }
}
