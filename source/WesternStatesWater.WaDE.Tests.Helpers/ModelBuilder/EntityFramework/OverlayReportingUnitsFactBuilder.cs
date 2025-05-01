using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class OverlayReportingUnitsFactBuilder
    {
        public static OverlayReportingUnitsFact Create()
        {
            return Create(new RegulatoryReportingUnitsFactBuilderOptions());
        }

        public static OverlayReportingUnitsFact Create(RegulatoryReportingUnitsFactBuilderOptions opts)
        {
            return new Faker<OverlayReportingUnitsFact>()
                .RuleFor(a => a.OrganizationId, f => opts.OrganizationsDim?.OrganizationId ?? OrganizationsDimBuilder.GenerateId())
                .RuleFor(a => a.OverlayId, f => opts.RegulatoryOverlay?.OverlayId ?? OverlayDimBuilder.GenerateId())
                .RuleFor(a => a.ReportingUnitId, f => opts.ReportingUnits?.ReportingUnitId ?? ReportingUnitsDimBuilder.GenerateId())
                .RuleFor(a => a.DataPublicationDateId, f => opts.DataPublicationDate?.DateId ?? DateDimBuilder.GenerateId());
        }

        public static async Task<OverlayReportingUnitsFact> Load(WaDEContext db)
        {
            return await Load(db, new RegulatoryReportingUnitsFactBuilderOptions());
        }

        public static async Task<OverlayReportingUnitsFact> Load(WaDEContext db, RegulatoryReportingUnitsFactBuilderOptions opts)
        {
            opts.OrganizationsDim ??= await OrganizationsDimBuilder.Load(db);
            opts.RegulatoryOverlay ??= await OverlayDimBuilder.Load(db);
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