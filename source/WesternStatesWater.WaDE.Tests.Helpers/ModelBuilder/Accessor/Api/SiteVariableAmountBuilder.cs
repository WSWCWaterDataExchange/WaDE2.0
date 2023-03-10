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
                .RuleFor(a => a.BeneficialUses, f => new List<string> { f.Random.Word() });

            return faker;
        }
    }
}