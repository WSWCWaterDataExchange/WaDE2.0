﻿using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class WaterQualityIndicatorBuilder
    {
        private static int _globalIndex = 0;
        public static WaterQualityIndicator Create()
        {
            return Create(new WaterQualityIndicatorBuilderOptions());
        }

        public static WaterQualityIndicator Create(WaterQualityIndicatorBuilderOptions opts)
        {
            return new Faker<WaterQualityIndicator>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<WaterQualityIndicator> Load(WaDEContext db)
        {
            return await Load(db, new WaterQualityIndicatorBuilderOptions());
        }

        public static async Task<WaterQualityIndicator> Load(WaDEContext db, WaterQualityIndicatorBuilderOptions opts)
        {
            var item = Create(opts);

            db.WaterQualityIndicator.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,100);
        }
    }

    public class WaterQualityIndicatorBuilderOptions
    {

    }
}