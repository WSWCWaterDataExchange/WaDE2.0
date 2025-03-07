﻿using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class CoordinateMethodBuilder
    {
        private static int _globalIndex = 0;
        public static CoordinateMethod Create()
        {
            return Create(new CoordinateMethodBuilderOptions());
        }

        public static CoordinateMethod Create(CoordinateMethodBuilderOptions opts)
        {
            var faker = new Faker<CoordinateMethod>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.AlphaNumeric(2))
                .RuleFor(a => a.Definition, f => f.Random.AlphaNumeric(10))
                .RuleFor(a => a.State, f => f.Random.AlphaNumeric(10))
                .RuleFor(a => a.SourceVocabularyUri, f => f.Random.AlphaNumeric(100));

            return faker;
        }

        public static async Task<CoordinateMethod> Load(WaDEContext db)
        {
            return await Load(db, new CoordinateMethodBuilderOptions());
        }

        public static async Task<CoordinateMethod> Load(WaDEContext db, CoordinateMethodBuilderOptions opts)
        {
            var item = Create(opts);

            db.CoordinateMethod.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,100);
        }
    }

    public class CoordinateMethodBuilderOptions
    {

    }
}