﻿using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{

    public static class SiteRelationshipFactBuilder
    {
        public static PODSiteToPOUSiteFact Create()
        {
            return Create();
        }

        public static PODSiteToPOUSiteFact Create(SiteRelationshipBuilderOptions opt)
        {
            var faker = new Faker<PODSiteToPOUSiteFact>()
                .RuleFor(a => a.PODSiteId, f => opt.PODSite ?? f.Random.Long(1))
                .RuleFor(a => a.POUSiteId, f => opt.POUSite ?? f.Random.Long(1))
                .RuleFor(a => a.StartDate, f => f.Date.Past(5))
                .RuleFor(a => a.EndDate, f => f.Date.Past(2));


            return faker;
        }

        public static async Task<PODSiteToPOUSiteFact> Load(WaDEContext db)
        {
            return await Load(db, new SiteRelationshipBuilderOptions());
        }

        public static async Task<PODSiteToPOUSiteFact> Load(WaDEContext db, SiteRelationshipBuilderOptions opts)
        {
            var item = Create(opts);

            db.PODSiteToPOUSiteFact.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

    }

    public class SiteRelationshipBuilderOptions
    {
        public long? PODSite { get; set; }
        public long? POUSite { get; set; }
    }
}
