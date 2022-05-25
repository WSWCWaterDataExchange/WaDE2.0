using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Accessors.EntityFramework;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{

    public static class PODSiteToPOUSiteFactBuilder
    {
        public static PODSiteToPOUSiteFact Create()
        {
            return Create();
        }

        public static PODSiteToPOUSiteFact Create(PODSiteToPOUSiteFactBuilderOptions opt)
        {
            var faker = new Faker<PODSiteToPOUSiteFact>()
                .RuleFor(a => a.PODSiteId, f => opt.PodSite?.SiteId)
                .RuleFor(a => a.POUSiteId, f => opt.PouSite?.SiteId)
                .RuleFor(a => a.StartDate, f => f.Date.Past(5))
                .RuleFor(a => a.EndDate, f => f.Date.Past(2));

            return faker;
        }

        public static async Task<PODSiteToPOUSiteFact> Load(WaDEContext db)
        {
            return await Load(db, new PODSiteToPOUSiteFactBuilderOptions());
        }

        public static async Task<PODSiteToPOUSiteFact> Load(WaDEContext db, PODSiteToPOUSiteFactBuilderOptions opts)
        {
            opts.PodSite ??= await SitesDimBuilder.Load(db);
            opts.PouSite ??= await SitesDimBuilder.Load(db);

            var item = Create(opts);

            db.PODSiteToPOUSiteFact.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
    }

    public class PODSiteToPOUSiteFactBuilderOptions
    {
        public SitesDim PodSite { get; set; }
        public SitesDim PouSite { get; set; }
    }
}
