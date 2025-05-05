using Bogus;
using System.Collections.Generic;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class RegulatoryReportingUnitsOrganizationBuilder
    {
        public static OverlayReportingUnitsOrganization Create()
        {
            var faker = new Faker<OverlayReportingUnitsOrganization>()
                .RuleFor(a => a.OrganizationId, f => f.Random.Long(1))
                .RuleFor(a => a.OrganizationName, f => f.Company.CompanyName())
                .RuleFor(a => a.OrganizationPurview, f => f.Random.Word())
                .RuleFor(a => a.OrganizationWebsite, f => f.Internet.Url())
                .RuleFor(a => a.OrganizationPhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(a => a.OrganizationContactName, f => f.Person.FullName)
                .RuleFor(a => a.OrganizationContactEmail, f => f.Internet.Email())
                .RuleFor(a => a.OrganizationState, f => f.Address.StateAbbr())

                .RuleFor(a => a.Overlays, f => new List<Overlay> { RegulatoryOverlayBuilder.Create() })
                .RuleFor(a => a.ReportingUnitsOverlay, f => new List<ReportingUnitOverlay> { ReportingUnitRegulatoryBuilder.Create() });

            return faker;
        }
    }
}