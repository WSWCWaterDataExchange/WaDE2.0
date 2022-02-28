using Bogus;
using System.Collections.Generic;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class SiteVariableAmountsOrganizationBuilder
    {
        public static SiteVariableAmountsOrganization Create()
        {
            var faker = new Faker<SiteVariableAmountsOrganization>()
                .RuleFor(a => a.OrganizationId, f => f.Random.Long(1))
                .RuleFor(a => a.OrganizationName, f => f.Company.CompanyName())
                .RuleFor(a => a.OrganizationPurview, f => f.Random.Word())
                .RuleFor(a => a.OrganizationWebsite, f => f.Internet.Url())
                .RuleFor(a => a.OrganizationPhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(a => a.OrganizationContactName, f => f.Person.FullName)
                .RuleFor(a => a.OrganizationContactEmail, f => f.Internet.Email())
                .RuleFor(a => a.OrganizationState, f => f.Address.StateAbbr())

                .RuleFor(a => a.WaterSources, f => new List<WaterSource> { WaterSourceBuilder.Create() })
                .RuleFor(a => a.VariableSpecifics, f => new List<VariableSpecific> { VariableSpecificBuilder.Create() })
                .RuleFor(a => a.Methods, f => new List<Method> { MethodBuilder.Create() })
                .RuleFor(a => a.BeneficialUses, f => new List<BeneficialUse> { BeneficialUseBuilder.Create() })
                .RuleFor(a => a.SiteVariableAmounts, f => new List<SiteVariableAmount> { SiteVariableAmountBuilder.Create() })
                .RuleFor(a => a.Sites, f => new List<Site> { SiteBuilder.Create() });

            return faker;
        }
    }
}