using System.Globalization;
using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import
{
    public static class OverlayReportingUnitBuilder
    {
        public static OverlayReportingUnits Create()
        {
            return Create(new OverlayReportingUnitBuilderOptions());
        }

        public static OverlayReportingUnits Create(OverlayReportingUnitBuilderOptions opts)
        {
            var faker = new Faker<OverlayReportingUnits>()
                .RuleFor(a => a.OrganizationUUID, f => opts?.Organization?.OrganizationUuid ?? f.Random.Uuid().ToString())
                .RuleFor(a => a.OverlayUUID, f => opts?.Overlay?.OverlayUuid ?? f.Random.Uuid().ToString())
                .RuleFor(a => a.ReportingUnitUUID, f => opts?.ReportingUnit?.ReportingUnitUuid ?? f.Random.Uuid().ToString())
                .RuleFor(a => a.DataPublicationDate, f => (opts?.DatePublication?.Date ?? f.Date.Past(10)).ToString(CultureInfo.InvariantCulture));

            return faker;
        }
    }

    public class OverlayReportingUnitBuilderOptions
    {
        public OrganizationsDim Organization { get; set; }
        public OverlayDim Overlay { get; set; }
        public ReportingUnitsDim ReportingUnit { get; set; }
        public DateDim DatePublication { get; set; }
    }
}
