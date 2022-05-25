using Bogus;
using System;
using System.Collections.Generic;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class AggregatedAmountBuilder
    {
        public static AggregatedAmount Create()
        {
            var faker = new Faker<AggregatedAmount>()
                .RuleFor(a => a.AggregatedAmountId, f => f.Random.Long(1))
                .RuleFor(a => a.Variable, f => f.Random.Word())
                .RuleFor(a => a.VariableSpecificTypeCV, f => f.Random.Word())
                .RuleFor(a => a.MethodUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.ReportYear, f => f.Random.Int(1850, DateTime.Now.Year).ToString())
                .RuleFor(a => a.TimeframeStart, f => f.PickRandom<DateTime?>(f.Date.Past(100), null))
                .RuleFor(a => a.TimeframeEnd, f => f.PickRandom<DateTime?>(f.Date.Past(100), null))
                .RuleFor(a => a.WaterSourceUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.ReportingUnitUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.Amount, f => f.Random.Double(0, 100000000))
                .RuleFor(a => a.PopulationServed, f => f.PickRandom<long?>(f.Random.Long(0, 10000000), null))
                .RuleFor(a => a.PowerGeneratedGWh, f => f.PickRandom<double?>(f.Random.Double(0, 100000000), null))
                .RuleFor(a => a.IrrigatedAcreage, f => f.PickRandom<double?>(f.Random.Double(0, 1000000), null))
                .RuleFor(a => a.DataPublicationDate, f => f.PickRandom<DateTime?>(f.Date.Past(100), null))
                .RuleFor(a => a.BeneficialUses, f => new List<string> { f.Random.Word() })
                .RuleFor(a => a.PrimaryUse, f => f.Random.Word());

            return faker;
        }
    }
}