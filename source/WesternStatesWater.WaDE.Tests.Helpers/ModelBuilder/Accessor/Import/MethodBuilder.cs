using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import
{
    public static class MethodBuilder
    {
        public static Method Create(MethodBuilderOptions opts)
        {
            var faker = new Faker<Method>()
                .RuleFor(a => a.MethodUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.MethodName, f => f.Random.Word(50))
                .RuleFor(a => a.MethodDescription, f => f.Random.Words(5))
                .RuleFor(a => a.MethodNEMILink, f => f.Internet.Url())
                .RuleFor(a => a.ApplicableResourceTypeCV, f => opts.ApplicableResourceType.Name)
                .RuleFor(a => a.MethodTypeCV, f => opts.MethodType.Name)
                .RuleFor(a => a.DataCoverageValue, f => f.Random.Word())
                .RuleFor(a => a.DataQualityValueCV, f => opts.DataQualityValue.Name)
                .RuleFor(a => a.DataConfidenceValue, f => f.Random.Word(50))
                .RuleFor(a => a.WaDEDataMappingUrl, f => f.Internet.Url());

            return faker;
        }
    }

    public class MethodBuilderOptions
    {
        public ApplicableResourceType ApplicableResourceType { get; set; }
        public MethodType MethodType { get; set; }
        public DataQualityValue DataQualityValue { get; set; }
    }
}