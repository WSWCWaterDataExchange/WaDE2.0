using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class RegulatoryReportingUnitsFactBuilder
    {
        public static RegulatoryReportingUnitsFact Create()
        {
            return Create(new RegulatoryReportingUnitsFactBuilderOptions());
        }

        public static RegulatoryReportingUnitsFact Create(RegulatoryReportingUnitsFactBuilderOptions opts)
        {
            return new Faker<RegulatoryReportingUnitsFact>()
                .RuleFor(a => a.OrganizationId, f => opts.OrganizationsDim?.OrganizationId ?? OrganizationsDimBuilder.GenerateId())
                .RuleFor(a => a.RegulatoryOverlayId, f => opts.RegulatoryOverlay?.OverlayId ?? RegulatoryOverlayDimBuilder.GenerateId())
                .RuleFor(a => a.ReportingUnitId, f => opts.ReportingUnits?.ReportingUnitId ?? ReportingUnitsDimBuilder.GenerateId())
                .RuleFor(a => a.DataPublicationDateId, f => opts.DataPublicationDate?.DateId ?? DateDimBuilder.GenerateId());
        }

        public static async Task<RegulatoryReportingUnitsFact> Load(WaDEContext db)
        {
            return await Load(db, new RegulatoryReportingUnitsFactBuilderOptions());
        }

        public static async Task<RegulatoryReportingUnitsFact> Load(WaDEContext db, RegulatoryReportingUnitsFactBuilderOptions opts)
        {
            opts.OrganizationsDim ??= await OrganizationsDimBuilder.Load(db);
            opts.RegulatoryOverlay ??= await RegulatoryOverlayDimBuilder.Load(db);
            opts.ReportingUnits ??= await ReportingUnitsDimBuilder.Load(db);
            opts.DataPublicationDate ??= await DateDimBuilder.Load(db);

            var item = Create(opts);

            db.OverlayReportingUnitsFact.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static string GenerateName()
        {
            return new Faker().Random.Word();
        }
    }

    public class RegulatoryReportingUnitsFactBuilderOptions
    {
        public OrganizationsDim OrganizationsDim { get; set; }
        public OverlayDim RegulatoryOverlay { get; set; }
        public ReportingUnitsDim ReportingUnits { get; set; }
        public DateDim DataPublicationDate { get; set; }
    }
}