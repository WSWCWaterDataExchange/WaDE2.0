﻿using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class VariableBuilder
    {
        private static int _globalIndex = 0;
        public static Variable Create()
        {
            return Create(new VariableBuilderOptions());
        }

        public static Variable Create(VariableBuilderOptions opts)
        {
            return new Faker<Variable>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.WaDEName, f => f.Random.Words(3))
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<Variable> Load(WaDEContext db)
        {
            return await Load(db, new VariableBuilderOptions());
        }

        public static async Task<Variable> Load(WaDEContext db, VariableBuilderOptions opts)
        {
            var item = Create(opts);

            db.Variable.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,250);
        }
    }

    public class VariableBuilderOptions
    {
    }
}