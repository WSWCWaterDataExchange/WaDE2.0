using Bogus;
using System;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class RegulatoryOverlayBuilder
    {
        public static Overlay Create()
        {
            var faker = new Faker<Overlay>()
                .RuleFor(a => a.OverlayID, f => f.Random.Long(1))
                .RuleFor(a => a.OverlayStatusCV, f => f.Random.Word(50))
                .RuleFor(a => a.OversightAgency, f => f.Company.CompanyName())
                .RuleFor(a => a.OverlayDescription, f => f.Lorem.Sentence())
                .RuleFor(a => a.StatutoryEffectiveDate, f => f.Date.Past(100))
                .RuleFor(a => a.StatutoryEndDate, (f, o) => f.Date.Between(o.StatutoryEffectiveDate, DateTime.Now))
                .RuleFor(a => a.StatuteLink, f => f.Internet.Url())
                .RuleFor(a => a.OverlayTypeCV, f => f.Random.Word())
                .RuleFor(a => a.WaterSourceTypeCV, f => f.Random.Word())
                .RuleFor(a => a.OverlayUUID, f => f.Random.Uuid().ToString());

            return faker;
        }
    }
}