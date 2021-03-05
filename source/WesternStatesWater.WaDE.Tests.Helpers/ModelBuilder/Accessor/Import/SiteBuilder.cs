using Bogus;
using NetTopologySuite;
using NetTopologySuite.IO;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import
{
    public static class SiteBuilder
    {
        public static Site Create()
        {
            return Create(new SiteBuilderOptions());
        }

        public static Site Create(SiteBuilderOptions opts)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            WKTReader shapeMaker = new WKTReader(geometryFactory);

            var faker = new Faker<Site>()
                .RuleFor(a => a.SiteUUID, f => opts?.Site?.SiteUuid ?? f.Random.Uuid().ToString())
                .RuleFor(a => a.SiteName, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.CoordinateMethodCV, f => opts.CoordinateMethodCvNavigation?.Name)
                .RuleFor(a => a.EPSGCodeCV, f => opts.EpsgcodeCvNavigation?.Name)
                .RuleFor(a => a.PODorPOUSite, f => f.Random.Word())
                ;

            return faker;
        }


    }

    public class SiteBuilderOptions
    {
        public SitesDim Site { get; set; }
        public CoordinateMethod CoordinateMethodCvNavigation { get; set; }
        public Epsgcode EpsgcodeCvNavigation { get; set; }
    }
}
