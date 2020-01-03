using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class CropTypeBuilder
    {
        public static CropType Create()
        {
            return Create(new CropTypeBuilderOptions());
        }

        public static CropType Create(CropTypeBuilderOptions opts)
        {
            return new Faker<CropType>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static string GenerateName()
        {
            return new Faker().Random.Word();
        }
    }

    public class CropTypeBuilderOptions
    {

    }
}