﻿using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class NhdnetworkStatusBuilder
    {
        private static int _globalIndex = 0;
        public static NhdnetworkStatus Create()
        {
            return Create(new NhdnetworkStatusBuilderOptions());
        }

        public static NhdnetworkStatus Create(NhdnetworkStatusBuilderOptions opts)
        {
            var faker = new Faker<NhdnetworkStatus>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.AlphaNumeric(250))
                .RuleFor(a => a.Definition, f => f.Random.AlphaNumeric(4000))
                .RuleFor(a => a.State, f => f.Random.AlphaNumeric(250))
                .RuleFor(a => a.SourceVocabularyUri, f => f.Random.AlphaNumeric(250));

            return faker;
        }

        public static async Task<NhdnetworkStatus> Load(WaDEContext db)
        {
            return await Load(db, new NhdnetworkStatusBuilderOptions());
        }

        public static async Task<NhdnetworkStatus> Load(WaDEContext db, NhdnetworkStatusBuilderOptions opts)
        {
            var item = Create(opts);

            db.NhdnetworkStatus.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,50);
        }
    }

    public class NhdnetworkStatusBuilderOptions
    {

    }
}