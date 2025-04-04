﻿using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class IrrigationMethodBuilder
    {
        private static int _globalIndex = 0;
        public static IrrigationMethod Create()
        {
            return Create(new IrrigationMethodBuilderOptions());
        }

        public static IrrigationMethod Create(IrrigationMethodBuilderOptions opts)
        {
            return new Faker<IrrigationMethod>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<IrrigationMethod> Load(WaDEContext db)
        {
            return await Load(db, new IrrigationMethodBuilderOptions());
        }

        public static async Task<IrrigationMethod> Load(WaDEContext db, IrrigationMethodBuilderOptions opts)
        {
            var item = Create(opts);

            db.IrrigationMethod.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,100);
        }
    }

    public class IrrigationMethodBuilderOptions
    {

    }
}