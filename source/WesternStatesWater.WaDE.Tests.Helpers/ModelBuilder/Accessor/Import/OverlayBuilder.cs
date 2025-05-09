﻿using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import
{
    public static class OverlayBuilder
    {
        public static Overlay Create()
        {
            return Create(new OverlayBuilderOptions());
        }

        public static Overlay Create(OverlayBuilderOptions opts)
        {
            var faker = new Faker<Overlay>()
                .RuleFor(a => a.OverlayUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.OverlayNativeID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.OverlayName, f => f.Company.CompanyName())
                .RuleFor(a => a.OverlayDescription, f => f.Rant.ToString())
                .RuleFor(a => a.OverlayStatusCV, f => opts?.OverlayStatus?.Name ?? f.Random.Word())
                .RuleFor(a => a.OversightAgency, f => f.Company.CompanyName())
                .RuleFor(a => a.Statute, f => f.Random.Word())
                .RuleFor(a => a.StatuteLink, f => f.Internet.Url())
                .RuleFor(a => a.StatutoryEffectiveDate, f => f.Date.Past(10))
                .RuleFor(a => a.StatutoryEndDate, f => f.Date.Past(5))
                .RuleFor(a => a.OverlayTypeCV, f => opts?.OverlayType?.Name ?? f.Random.AlphaNumeric(10))
                .RuleFor(a => a.WaterSourceTypeCV, f => opts?.WaterSourceType?.Name ?? f.Random.AlphaNumeric(10));

            return faker;
        }
    }

    public class OverlayBuilderOptions
    {
        public OverlayStatus OverlayStatus { get; set; }
        public OverlayTypeCV OverlayType { get; set; }
        public WaterSourceType WaterSourceType { get; set; }
    }
}
