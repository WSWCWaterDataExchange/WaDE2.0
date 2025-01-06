using System.Threading.Tasks;
using Bogus;
using NetTopologySuite.Geometries;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class ReportingUnitsDimBuilder
    {
        public static ReportingUnitsDim Create()
        {
            return Create(new ReportingUnitsDimBuilderOptions());
        }

        public static ReportingUnitsDim Create(ReportingUnitsDimBuilderOptions opts)
        {
            return new Faker<ReportingUnitsDim>()
                .RuleFor(a => a.ReportingUnitUuid, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.ReportingUnitNativeId, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.ReportingUnitName, f => f.Company.CompanyName())
                .RuleFor(a => a.ReportingUnitTypeCv, f => opts.ReportingUnitType?.Name ?? ReportingUnitTypeBuilder.GenerateName())
                .RuleFor(a => a.ReportingUnitUpdateDate, f => f.Date.Past())
                .RuleFor(a => a.ReportingUnitProductVersion, f => f.System.Version().ToString())
                .RuleFor(a => a.StateCv, f => opts.State?.Name ?? f.Address.StateAbbr())
                .RuleFor(a => a.EpsgcodeCv, f => opts.Epsgcode?.Name ?? f.Random.AlphaNumeric(10))
                .RuleFor(a => a.Geometry, f => opts?.Geometry);
        }

        public static async Task<ReportingUnitsDim> Load(WaDEContext db)
        {
            return await Load(db, new ReportingUnitsDimBuilderOptions());
        }

        public static async Task<ReportingUnitsDim> Load(WaDEContext db, ReportingUnitsDimBuilderOptions opts)
        {
            opts.ReportingUnitType = opts.ReportingUnitType ?? await ReportingUnitTypeBuilder.Load(db);
            opts.State = opts.State ?? await StateBuilder.Load(db);
            opts.Epsgcode = opts.Epsgcode ?? await EpsgcodeBuilder.Load(db);

            var item = Create(opts);

            db.ReportingUnitsDim.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static long GenerateId()
        {
            return new Faker().Random.Long(1);
        }
    }

    public class ReportingUnitsDimBuilderOptions
    {
        public ReportingUnitType ReportingUnitType { get; set; }
        public State State { get; set; }
        public Epsgcode Epsgcode { get; set; }
        public Geometry Geometry { get; set; }
    }
}