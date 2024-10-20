﻿using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class OwnerClassificationBuilder
    {
        public static OwnerClassificationCv Create()
        {
            return Create(new OwnerClassificationBuilderOptions());
        }

        public static OwnerClassificationCv Create(OwnerClassificationBuilderOptions opts)
        {
            return new Faker<OwnerClassificationCv>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyURI, f => f.Internet.Url());
        }

        public static async Task<OwnerClassificationCv> Load(WaDEContext db)
        {
            return await Load(db, new OwnerClassificationBuilderOptions());
        }

        public static async Task<OwnerClassificationCv> Load(WaDEContext db, OwnerClassificationBuilderOptions opts)
        {
            var item = Create(opts);

            db.OwnerClassificationCv.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static string GenerateName()
        {
            return new Faker().Random.AlphaNumeric(50);
        }
    }

    public class OwnerClassificationBuilderOptions
    {

    }
}
