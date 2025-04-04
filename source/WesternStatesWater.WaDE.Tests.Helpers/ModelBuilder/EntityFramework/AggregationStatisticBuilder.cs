﻿using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class AggregationStatisticBuilder
    {
        private static int _globalIndex = 0;
        public static AggregationStatistic Create()
        {
            return Create(new AggregationStatisticBuilderOptions());
        }

        public static AggregationStatistic Create(AggregationStatisticBuilderOptions opts)
        {
            return new Faker<AggregationStatistic>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<AggregationStatistic> Load(WaDEContext db)
        {
            return await Load(db, new AggregationStatisticBuilderOptions());
        }

        public static async Task<AggregationStatistic> Load(WaDEContext db, AggregationStatisticBuilderOptions opts)
        {
            var item = Create(opts);

            db.AggregationStatistic.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,50);
        }
    }

    public class AggregationStatisticBuilderOptions
    {

    }
}