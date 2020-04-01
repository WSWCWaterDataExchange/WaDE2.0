using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.EntityFrameworkCore.Internal;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class DateDimBuilder
    {
        public static DateDim Create()
        {
            return Create(new DateDimBuilderOptions());
        }

        public static DateDim Create(DateDimBuilderOptions opts)
        {
            return new Faker<DateDim>()
                .RuleFor(a => a.Date, f => f.Date.Past(50))
                .RuleFor(a=>a.Year, (f,u)=>u.Date.Year.ToString());
        }

        public static async Task<DateDim> Load(WaDEContext db)
        {
            return await Load(db, new DateDimBuilderOptions());
        }

        public static async Task<DateDim> Load(WaDEContext db, DateDimBuilderOptions opts)
        {
            var item = Create(opts);

            var matchingDate = db.DateDim.FirstOrDefault(a => a.Date == item.Date);

            if (matchingDate == null)
            {
                db.DateDim.Add(item);
                await db.SaveChangesAsync();
                matchingDate = item;
            }

            return matchingDate;
        }

        public static long GenerateId()
        {
            return (new Faker()).Random.Long(1);
        }
    }

    public class DateDimBuilderOptions
    {
    }
}