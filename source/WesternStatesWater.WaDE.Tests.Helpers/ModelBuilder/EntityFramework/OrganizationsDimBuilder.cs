using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class OrganizationsDimBuilder
    {
        public static OrganizationsDim Create()
        {
            return Create(new OrganizationsDimBuilderOptions());
        }

        public static OrganizationsDim Create(OrganizationsDimBuilderOptions opts)
        {
            return new Faker<OrganizationsDim>()
                .RuleFor(a => a.OrganizationUuid, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.OrganizationName, f => f.Company.CompanyName())
                .RuleFor(a => a.OrganizationPurview, f => f.Random.Word())
                .RuleFor(a => a.OrganizationWebsite, f => f.Internet.Url())
                .RuleFor(a => a.OrganizationPhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(a => a.OrganizationContactName, f => f.Name.FullName())
                .RuleFor(a => a.OrganizationContactEmail, f => f.Internet.Email())
                .RuleFor(a => a.OrganizationDataMappingUrl, f => f.Internet.Url())
                .RuleFor(a => a.State, f => opts.State?.Name ?? f.Address.StateAbbr());
        }

        public static async Task<OrganizationsDim> Load(WaDEContext db)
        {
            return await Load(db, new OrganizationsDimBuilderOptions());
        }

        public static async Task<OrganizationsDim> Load(WaDEContext db, OrganizationsDimBuilderOptions opts)
        {
            opts.State = opts.State ?? await StateBuilder.Load(db);
            var item = Create(opts);

            db.OrganizationsDim.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static long GenerateId()
        {
            return new Faker().Random.Long(1);
        }
    }

    public class OrganizationsDimBuilderOptions
    {
        public State State { get; set; }
    }
}
