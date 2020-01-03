using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class MethodsDimBuilder
    {
        public static MethodsDim Create()
        {
            return Create(new MethodsDimBuilderOptions());
        }

        public static MethodsDim Create(MethodsDimBuilderOptions opts)
        {
            return new Faker<MethodsDim>()
                .RuleFor(a => a.MethodUuid, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.MethodName, f => f.Random.Word())
                .RuleFor(a => a.MethodDescription, f => f.Random.Words(5))
                .RuleFor(a => a.MethodNemilink, f => f.Internet.Url())
                .RuleFor(a => a.ApplicableResourceTypeCv, f => opts.ApplicableResourceType?.Name ?? ApplicableResourceTypeBuilder.GenerateName())
                .RuleFor(a => a.MethodTypeCv, f => opts.MethodType?.Name ?? MethodTypeBuilder.GenerateName())
                .RuleFor(a => a.DataCoverageValue, f => f.Random.Word())
                .RuleFor(a => a.DataQualityValueCv, f => opts.DataQualityValue?.Name)
                .RuleFor(a => a.DataConfidenceValue, f => f.Random.Word());
        }

        public static async Task<MethodsDim> Load(WaDEContext db)
        {
            return await Load(db, new MethodsDimBuilderOptions());
        }

        public static async Task<MethodsDim> Load(WaDEContext db, MethodsDimBuilderOptions opts)
        {
            opts.ApplicableResourceType = opts.ApplicableResourceType ?? await ApplicableResourceTypeBuilder.Load(db);
            opts.MethodType = opts.MethodType ?? await MethodTypeBuilder.Load(db);

            var item = Create(opts);

            db.MethodsDim.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static long GenerateId()
        {
            return new Faker().Random.Long(1);
        }
    }

    public class MethodsDimBuilderOptions
    {
        public ApplicableResourceType ApplicableResourceType { get; set; }
        public MethodType MethodType { get; set; }
        public DataQualityValue DataQualityValue { get; set; }
    }
}