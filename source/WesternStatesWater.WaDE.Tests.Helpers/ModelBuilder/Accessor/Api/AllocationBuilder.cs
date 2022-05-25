using Bogus;
using System;
using System.Collections.Generic;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class AllocationBuilder
    {
        public static Allocation Create()
        {
            var faker = new Faker<Allocation>()
                .RuleFor(a => a.AllocationAmountId, f => f.Random.Long(1))
                .RuleFor(a => a.AllocationNativeID, f => f.Random.AlphaNumeric(50))
                .RuleFor(a => a.AllocationOwner, f => f.Person.FullName)
                .RuleFor(a => a.AllocationApplicationDate, f => f.PickRandom<DateTime?>(f.Date.Past(100), null))
                .RuleFor(a => a.AllocationPriorityDate, f => f.PickRandom<DateTime?>(f.Date.Past(100), null))
                .RuleFor(a => a.AllocationLegalStatusCodeCV, f => f.Random.Word())
                .RuleFor(a => a.AllocationExpirationDate, f => f.PickRandom<DateTime?>(f.Date.Past(100), null))
                .RuleFor(a => a.AllocationChangeApplicationIndicator, f => f.Random.Word())
                .RuleFor(a => a.LegacyAllocationIDs, f => f.Random.Word())
                .RuleFor(a => a.AllocationAcreage, f => f.PickRandom<double?>(f.Random.Double(1, 10000), null))
                .RuleFor(a => a.AllocationBasisCV, f => f.Random.Word())
                .RuleFor(a => a.TimeframeStart, f => f.PickRandom<DateTime?>(f.Date.Past(100), null))
                .RuleFor(a => a.TimeframeEnd, f => f.PickRandom<DateTime?>(f.Date.Past(100), null))
                .RuleFor(a => a.DataPublicationDate, f => f.PickRandom<DateTime?>(f.Date.Past(100), null))
                .RuleFor(a => a.AllocationCropDutyAmount, f => f.PickRandom<double?>(f.Random.Double(1, 10000), null))
                .RuleFor(a => a.AllocationFlow_CFS, f => f.PickRandom<double?>(f.Random.Double(1, 100), null))
                .RuleFor(a => a.AllocationVolume_AF, f => f.PickRandom<double?>(f.Random.Double(1, 10000), null))
                .RuleFor(a => a.PopulationServed, f => f.PickRandom<long?>(f.Random.Long(1, 10000000), null))
                .RuleFor(a => a.GeneratedPowerCapacityMW, f => f.PickRandom<double?>(f.Random.Double(1, 100000), null))
                .RuleFor(a => a.AllocationCommunityWaterSupplySystem, f => f.Random.Word())
                .RuleFor(a => a.MethodId, f => f.Random.Long(1))
                .RuleFor(a => a.MethodUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.VariableSpecificId, f => f.Random.Long(1))
                .RuleFor(a => a.VariableSpecificTypeCV, f => f.Random.Word())
                .RuleFor(a => a.OrganizationId, f => f.Random.Long(1))
                .RuleFor(a => a.ExemptOfVolumeFlowPriority, f => f.Random.Bool())
                .RuleFor(a => a.SitesUUIDs, f => new List<string> { f.Random.Uuid().ToString() })
                .RuleFor(a => a.BeneficialUses, f => new List<string> { f.Random.Word() });

            return faker;
        }
    }
}