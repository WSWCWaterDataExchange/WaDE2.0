using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class MethodBuilder
    {
        public static Method Create()
        {
            var faker = new Faker<Method>()
                .RuleFor(a => a.MethodId, f => f.Random.Long(1))
                .RuleFor(a => a.MethodUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.MethodName, f => f.Random.Word())
                .RuleFor(a => a.MethodDescription, f => f.Rant.Review())
                .RuleFor(a => a.MethodNEMILink, f => f.Random.Word())
                .RuleFor(a => a.ApplicableResourceType, f => f.Random.Word())
                .RuleFor(a => a.MethodTypeCV, f => f.Random.Word())
                .RuleFor(a => a.DataCoverageValue, f => f.Random.Word())
                .RuleFor(a => a.DataQualityValue, f => f.Random.Word())
                .RuleFor(a => a.DataConfidenceValue, f => f.Random.Word());

            return faker;
        }
    }
}