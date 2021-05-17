using System;
using System.Collections.Generic;
using System.Text;
using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import
{

    public static class SiteRelationshipBuilder
    {
        public static PODSitePOUSite Create()
        {
            return Create();
        }

        public static PODSitePOUSite Create(SiteRelationshipBuilderOptions opt)
        {
            var faker = new Faker<PODSitePOUSite>()
                .RuleFor(a => a.PODSiteUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.POUSiteUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.StartDate, f => f.Date.Past(5))
                .RuleFor(a => a.EndDate, f => f.Date.Past(2))
                //.RuleFor(a => a.StartDate, f => f.Random.Uuid().ToString())
                //.RuleFor(a => a.WaterSourceUUID, f => f.Random.Uuid().ToString())
                //.RuleFor(a => a.MethodUUID, f => f.Random.Uuid().ToString())
                //.RuleFor(a => a.DataPublicationDate, f => f.Date.Past(5))
                //.RuleFor(a => a.DataPublicationDOI, f => f.Random.Words(5))
                //.RuleFor(a => a.AllocationNativeID, f => f.Random.Uuid().ToString())

                //.RuleFor(a => a.AllocationExpirationDate, f => f.Date.Past(5))
                //.RuleFor(a => a.AllocationOwner, f => f.Name.FullName())
                //.RuleFor(a => a.AllocationTimeframeStart, f => f.Random.AlphaNumeric(5))
                //.RuleFor(a => a.AllocationTimeframeEnd, f => f.Random.AlphaNumeric(5))
                //.RuleFor(a => a.AllocationFlow_CFS, f => f.Random.Double(0, 10000).ToString())
                //.RuleFor(a => a.AllocationVolume_AF, f => f.Random.Double(0, 10000).ToString())
                //.RuleFor(a => a.AllocationCommunityWaterSupplySystem, f => f.Address.City())
                ;

            //switch (opts.RecordType ?? (new Faker()).PickRandom<SiteRelationshipBuilderRecordType>())
            //{
            //    case SiteRelationshipBuilderRecordType.Ag:
            //        faker.RuleFor(a => a.IrrigatedAcreage, f => f.Random.Double(0, 10000).ToString())
            //            .RuleFor(a => a.AllocationCropDutyAmount, f => f.Random.Double(0, 50000).ToString());
            //        break;
            //    case SiteRelationshipBuilderRecordType.Power:
            //        faker.RuleFor(a => a.GeneratedPowerCapacityMW, f => f.Random.Double(0, 10000).ToString());
            //        break;
            //    case SiteRelationshipBuilderRecordType.Civilian:
            //        faker.RuleFor(a => a.PopulationServed, f => f.Random.Long(0, 10000000).ToString())
            //            .RuleFor(a => a.CommunityWaterSupplySystem, f => f.Address.City());
            //        break;
            //}

            return faker;
        }
    }

    public class SiteRelationshipBuilderOptions
    {
        public string site1 { get; set; }
        public string site2 { get; set; }
    }

//    public enum SiteRelationshipBuilderRecordType
//    {
//        None,
//        Power,
//        Civilian,
//        Ag
//    }
}
