using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class CustomerTypeBuilder
    {
        private static int _globalIndex = 0;
        public static CustomerType Create()
        {
            return Create(new CustomerTypeBuilderOptions());
        }

        public static CustomerType Create(CustomerTypeBuilderOptions opts)
        {
            return new Faker<CustomerType>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<CustomerType> Load(WaDEContext db)
        {
            return await Load(db, new CustomerTypeBuilderOptions());
        }

        public static async Task<CustomerType> Load(WaDEContext db, CustomerTypeBuilderOptions opts)
        {
            var item = Create(opts);

            db.CustomerType.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,100);
        }
    }

    public class CustomerTypeBuilderOptions
    {

    }
}