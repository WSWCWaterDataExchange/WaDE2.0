using Bogus;
using System;
using System.Collections.Generic;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class SiteVariableAmountBuilder
    {
        public static SiteVariableAmount Create()
        {
            var faker = new Faker<SiteVariableAmount>()
                .RuleFor(a => a.SiteVariableAmountId, f => f.Random.Long(1))
                .RuleFor(a => a.SiteName, f => f.Random.Word())
                .RuleFor(a => a.NativeSiteID, f => f.Random.AlphaNumeric(50))
                .RuleFor(a => a.SiteTypeCV, f => f.Random.Word())
                .RuleFor(a => a.Longitude, f => f.PickRandom<double?>(f.Random.Double(-179, 179), null))
                .RuleFor(a => a.Latitude, f => f.PickRandom<double?>(f.Random.Double(-90, 90), null))
                .RuleFor(a => a.SiteGeometry, f => f.Geography().Geometry())
                .RuleFor(a => a.CoordinateMethodCV, f => f.Random.Word())
                .RuleFor(a => a.AllocationGNISIDCV, f => f.Random.Word())
                .RuleFor(a => a.TimeframeStart, f => f.PickRandom<DateTime?>(f.Date.Past(100), null))
                .RuleFor(a => a.TimeframeEnd, f => f.PickRandom<DateTime?>(f.Date.Past(100), null))
                .RuleFor(a => a.DataPublicationDate, f => f.PickRandom<DateTime?>(f.Date.Past(100), null))
                .RuleFor(a => a.AllocationCropDutyAmount, f => f.PickRandom<double?>(f.Random.Double(1, 10000), null))
                .RuleFor(a => a.Amount, f => f.PickRandom<double?>(f.Random.Double(1, 10000), null))
                .RuleFor(a => a.IrrigationMethodCV, f => f.Random.Word())
                .RuleFor(a => a.IrrigatedAcreage, f => f.PickRandom<double?>(f.Random.Double(1, 10000), null))
                .RuleFor(a => a.CropTypeCV, f => f.Random.Word())
                .RuleFor(a => a.PopulationServed, f => f.PickRandom<long?>(f.Random.Long(1, 10000000), null))
                .RuleFor(a => a.PowerGeneratedGWh, f => f.PickRandom<double?>(f.Random.Double(1, 100000), null))
                .RuleFor(a => a.AllocationCommunityWaterSupplySystem, f => f.Random.Word())
                .RuleFor(a => a.SDWISIdentifier, f => f.Random.Word())
                .RuleFor(a => a.DataPublicationDOI, f => f.Random.Word())
                .RuleFor(a => a.ReportYearCV, f => f.Random.Int(1850, DateTime.Now.Year).ToString())
                .RuleFor(a => a.MethodUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.VariableSpecificUUID, f => f.Random.Word())
                .RuleFor(a => a.SiteUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.AssociatedNativeAllocationIDs, f => f.Random.AlphaNumeric(50))
                .RuleFor(a => a.BeneficialUses, f => new List<string> { f.Random.Word() })
                .RuleFor(a => a.HUC8, f => f.Random.Word())
                .RuleFor(a => a.HUC12, f => f.Random.Word())
                .RuleFor(a => a.County, f => f.Address.County());

            return faker;
        }
    }
}