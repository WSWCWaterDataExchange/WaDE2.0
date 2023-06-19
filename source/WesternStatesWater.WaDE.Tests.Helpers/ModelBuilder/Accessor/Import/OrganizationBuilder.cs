using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import
{
    public static class OrganizationBuilder
    {
        public static Organization Create()
        {
            var faker = new Faker<Organization>()
                .RuleFor(a => a.OrganizationUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.OrganizationName, f => f.Company.CompanyName())
                .RuleFor(a => a.OrganizationPurview, f => f.Random.Word())
                .RuleFor(a => a.OrganizationWebsite, f => f.Internet.Url())
                .RuleFor(a => a.OrganizationPhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(a => a.OrganizationContactName, f => f.Name.FullName())
                .RuleFor(a => a.OrganizationContactEmail, f => f.Internet.Email())
                .RuleFor(a => a.State, f => f.Address.StateAbbr());

            return faker;
        }
    }
}