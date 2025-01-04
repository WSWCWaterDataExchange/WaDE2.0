using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Tests.Helpers;

public static class SDWISIdentifierBuilder
{
    private static int _globalIndex = 0;
    public static SDWISIdentifier Create()
    {
        return Create(new SDWISIdentifierBuilderOptions());
    }

    public static SDWISIdentifier Create(SDWISIdentifierBuilderOptions opts)
    {
        return new Faker<SDWISIdentifier>()
            .RuleFor(a => a.Name, f => GenerateName())
            .RuleFor(a => a.Term, f => f.Random.Word())
            .RuleFor(a => a.Definition, f => f.Random.Words(5))
            .RuleFor(a => a.State, f => f.Address.StateAbbr())
            .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
    }

    public static async Task<SDWISIdentifier> Load(WaDEContext db)
    {
        return await Load(db, new SDWISIdentifierBuilderOptions());
    }

    public static async Task<SDWISIdentifier> Load(WaDEContext db, SDWISIdentifierBuilderOptions opts)
    {
        var item = Create(opts);

        db.SDWISIdentifier.Add(item);
        await db.SaveChangesAsync();

        return item;
    }

    public static string GenerateName()
    {
        _globalIndex++;
        return CvNameGenerator.GetNextName(_globalIndex,100);
    }
}

public class SDWISIdentifierBuilderOptions
{

}