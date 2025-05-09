﻿using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class WaterSourceTypeBuilder
    {
        private static int _globalIndex = 0;
        public static WaterSourceType Create()
        {
            return Create(new WaterSourceTypeBuilderOptions());
        }

        public static WaterSourceType Create(WaterSourceTypeBuilderOptions opts)
        {
            return new Faker<WaterSourceType>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.WaDEName, f => f.Random.Words(3))
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<WaterSourceType> Load(WaDEContext db)
        {
            return await Load(db, new WaterSourceTypeBuilderOptions());
        }

        public static async Task<WaterSourceType> Load(WaDEContext db, WaterSourceTypeBuilderOptions opts)
        {
            var item = Create(opts);

            db.WaterSourceType.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,100);
        }
    }

    public class WaterSourceTypeBuilderOptions
    {

    }
}