using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import
{

    public static class AggregatedAmountBuilder
    {
        public static AggregatedAmount Create()
        {
            return Create(new AggregatedAmountBuilderOptions());
        }

        public static AggregatedAmount Create(AggregatedAmountBuilderOptions opts)
        {
            var faker = new Faker<AggregatedAmount>()
                .RuleFor(a => a.OrganizationUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.VariableSpecificUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.WaterSourceUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.MethodUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.DataPublicationDate, f => f.Date.Past(5))
                .RuleFor(a => a.DataPublicationDOI, f => f.Random.Words(2))
                .RuleFor(a => a.Amount, f => f.Random.Number(int.MaxValue).ToString())
                ;

            return faker;
        }
    }

    public class AggregatedAmountBuilderOptions
    {
    }
}
