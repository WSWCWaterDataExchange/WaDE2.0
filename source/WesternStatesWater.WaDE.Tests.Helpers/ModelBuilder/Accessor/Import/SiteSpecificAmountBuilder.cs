using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import
{
    public static class SiteSpecificAmountBuilder
    {
        public static SiteSpecificAmount Create()
        {
            return Create(new SiteSpecificAmountBuilderOptions());
        }

        public static SiteSpecificAmount Create(SiteSpecificAmountBuilderOptions opts)
        {
            var faker = new Faker<SiteSpecificAmount>()
                .RuleFor(a => a.MethodUUID, f => opts?.Method?.MethodUuid ?? f.Random.Uuid().ToString())
                .RuleFor(a => a.OrganizationUUID, f => opts?.Organization?.OrganizationUuid ?? f.Random.Uuid().ToString())
                .RuleFor(a => a.SiteUUID, f => opts?.Site?.SiteUuid ?? f.Random.Uuid().ToString())
                .RuleFor(a => a.VariableSpecificUUID, f => opts?.Variable?.VariableSpecificUuid ?? f.Random.Uuid().ToString())
                .RuleFor(a => a.WaterSourceUUID, f => opts?.WaterSource?.WaterSourceUuid ?? f.Random.Uuid().ToString())
                .RuleFor(a => a.DataPublicationDate, f => opts?.DataPublicationDate?.Date ?? f.Date.Past(5))
                .RuleFor(a => a.ReportYearCV, f => opts?.ReportYear?.Name ?? f.Random.Word())
                .RuleFor(a => a.BeneficialUseCategory, f => opts?.BeneficialUse?.Name ?? f.Random.Word())
                .RuleFor(a => a.PrimaryUseCategory, f => opts?.PrimaryUseCategory?.Name ?? f.Random.Word())
                .RuleFor(a => a.TimeframeStart, f => f.Date.Past(5))
                .RuleFor(a => a.TimeframeEnd, f => f.Date.Past(5))
                .RuleFor(a => a.Amount, f => f.Random.Double(0, double.MaxValue).ToString())
                .RuleFor(a => a.AssociatedNativeAllocationIDs, f => f.Random.Uuid().ToString())
                ;

            switch (opts?.RecordType ?? (new Faker().PickRandom<SiteSpecificRecordType>()))
            {
                case SiteSpecificRecordType.Civilian:
                    faker
                        .RuleFor(a => a.PopulationServed, f => f.Random.Number(10000).ToString())
                        .RuleFor(a => a.CommunityWaterSupplySystem, f => f.Random.Word())
                        .RuleFor(a => a.CustomerTypeCV, f => opts?.CustomerType?.Name ?? f.Random.Word())
                        .RuleFor(a => a.SDWISIdentifier, f => opts?.SDWISIdentifier?.Name ?? f.Random.Word())
                        ;
                    break;

                case SiteSpecificRecordType.Ag:
                    faker
                        .RuleFor(a => a.IrrigatedAcreage, f => f.Random.Double(0, 10000).ToString())
                        .RuleFor(a => a.CropTypeCV, f => opts?.CropType?.Name ?? f.Random.Word())
                        .RuleFor(a => a.IrrigationMethodCV, f => opts?.IrrigationMethod?.Name ?? f.Random.Word())
                        .RuleFor(a => a.AllocationCropDutyAmount, f => f.Random.Double(0, 10000).ToString())
                        ;
                    break;

                case SiteSpecificRecordType.Power:
                    faker
                        .RuleFor(a => a.PowerGeneratedGWh, f => f.Random.Number(1000000).ToString())
                        .RuleFor(a => a.PowerType, f => opts?.PowerType?.Name ?? f.Random.Word())
                        ;
                    break;
            }

            return faker;
        }


    }

    public class SiteSpecificAmountBuilderOptions
    {
        public SiteSpecificRecordType? RecordType { get; set; }
        public MethodsDim Method { get; set; }
        public OrganizationsDim Organization { get; set; }
        public SitesDim Site { get; set; }
        public VariablesDim Variable { get; set; }
        public WaterSourcesDim WaterSource { get; set; }
        public DateDim DataPublicationDate { get; set; }
        public ReportYearCv ReportYear { get; set; }
        public IrrigationMethod IrrigationMethod { get; set; }
        public CropType CropType { get; set; }
        public CustomerType CustomerType { get; set; }
        public BeneficialUsesCV BeneficialUse { get; set; }
        public BeneficialUsesCV PrimaryUseCategory { get; set; }
        public SDWISIdentifier SDWISIdentifier { get; set; }
        public PowerType PowerType { get; set; }
    }

    public enum SiteSpecificRecordType
    {
        Ag,
        Civilian,
        Power
    }
}
