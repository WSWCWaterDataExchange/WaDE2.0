using Bogus;
using System;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class RegulatoryOverlayBuilder
    {
        public static RegulatoryOverlay Create()
        {
            var faker = new Faker<RegulatoryOverlay>()
                .RuleFor(a => a.RegulatoryOverlayID, f => f.Random.Long(1))
                .RuleFor(a => a.RegulatoryStatusCV, f => f.Random.Word())
                .RuleFor(a => a.OversightAgency, f => f.Company.CompanyName())
                .RuleFor(a => a.RegulatoryDescription, f => f.Lorem.Sentence())
                .RuleFor(a => a.StatutoryEffectiveDate, f => f.Date.Past(100))
                .RuleFor(a => a.StatutoryEndDate, (f, o) => f.Date.Between(o.StatutoryEffectiveDate, DateTime.Now))
                .RuleFor(a => a.RegulatoryStatuteLink, f => f.Internet.Url())
                .RuleFor(a => a.RegulatoryOverlayTypeCV, f => f.Random.Word())
                .RuleFor(a => a.WaterSourceTypeCV, f => f.Random.Word())
                .RuleFor(a => a.RegulatoryOverlayUUID, f => f.Random.Uuid().ToString());

            return faker;
        }
    }
}