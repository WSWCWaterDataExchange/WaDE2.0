using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class BeneficialUseBuilder
    {
        public static BeneficialUse Create()
        {
            var faker = new Faker<BeneficialUse>()
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.Definition, f => f.Lorem.Sentence())
                .RuleFor(a => a.Name, f => f.Random.Word())
                .RuleFor(a => a.SourceVocabularyURI, f => f.Internet.Url())
                .RuleFor(a => a.USGSCategory, f => f.Random.Word())
                .RuleFor(a => a.NAICSCode, f => f.Random.Word());

            return faker;
        }
    }
}