using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class AllocationAmountsFactBuilder
    {
        public static AllocationAmountsFact Create()
        {
            return Create(new AllocationAmountsFactBuilderOptions());
        }

        public static AllocationAmountsFact Create(AllocationAmountsFactBuilderOptions opts)
        {
            var faker = new Faker<AllocationAmountsFact>()
                .RuleFor(a => a.OrganizationId, f => opts.OrganizationsDim?.OrganizationId ?? OrganizationsDimBuilder.GenerateId())
                .RuleFor(a => a.VariableSpecificId, f => opts.VariablesDim?.VariableSpecificId ?? VariablesDimBuilder.GenerateId())
                .RuleFor(a => a.MethodId, f => opts.MethodsDim?.MethodId ?? MethodsDimBuilder.GenerateId())
                .RuleFor(a => a.PrimaryUseCategoryCV, f => opts.PrimaryBeneficialUsesCv?.Name)
                .RuleFor(a => a.DataPublicationDateId, f => opts.DataPublicationDate?.DateId ?? DateDimBuilder.GenerateId())
                .RuleFor(a => a.DataPublicationDoi, f => f.Random.Words(2))
                .RuleFor(a => a.AllocationNativeId, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.DataPublicationDoi, f => f.Random.Word())
                .RuleFor(a => a.AllocationApplicationDateID, f => opts.AllocationApplicationDate?.DateId)
                .RuleFor(a => a.AllocationPriorityDateID, f => opts.AllocationPriorityDate?.DateId ?? DateDimBuilder.GenerateId())
                .RuleFor(a => a.AllocationExpirationDateID, f => opts.AllocationExpirationDate?.DateId)
                .RuleFor(a => a.AllocationOwner, f => f.Name.FullName())
                .RuleFor(a => a.AllocationBasisCv, f => opts.WaterAllocationBasis?.Name)
                .RuleFor(a => a.AllocationLegalStatusCv, f => opts.LegalStatus?.Name)
                .RuleFor(a => a.AllocationTypeCv, f => opts.WaterAllocationType?.Name)
                .RuleFor(a => a.AllocationTimeframeEnd, f => opts.AllocationTimeframeStartDate?.Date.ToString("mm/dd"))
                .RuleFor(a => a.AllocationTimeframeEnd, f => opts.AllocationTimeframeEndDate?.Date.ToString("mm/dd"))
                .RuleFor(a => a.AllocationFlow_CFS, f => f.Random.Double(0, 1000))
                .RuleFor(a => a.AllocationVolume_AF, f => f.Random.Double(0, 1000))
                ;

            switch (opts.RecordType ?? (new Faker()).PickRandom<WaterAllocationRecordType>())
            {
                case WaterAllocationRecordType.Ag:
                    faker.RuleFor(a => a.IrrigatedAcreage, f => f.Random.Double(0, 10000))
                        .RuleFor(a => a.CropTypeCV, f => opts.CropType?.Name)
                        .RuleFor(a => a.IrrigationMethodCV, f => opts.IrrigationMethod?.Name)
                        .RuleFor(a => a.AllocationCropDutyAmount, f => f.Random.Double(0, 1000));
                    break;
                case WaterAllocationRecordType.Power:
                    faker.RuleFor(a => a.GeneratedPowerCapacityMW, f => f.Random.Double(0, 10000));
                    break;
                case WaterAllocationRecordType.Civilian:
                    faker.RuleFor(a => a.PopulationServed, f => f.Random.Long(0, 10000000))
                        .RuleFor(a => a.CommunityWaterSupplySystem, f => f.Address.City())
                        .RuleFor(a => a.CustomerTypeCV, f => opts.CustomerType?.Name)
                        .RuleFor(a => a.SdwisidentifierCV, f => opts.SDWISIdentifier?.Name);
                    break;
            }

            return faker;
        }

        public static async Task<AllocationAmountsFact> Load(WaDEContext db)
        {
            return await Load(db, new AllocationAmountsFactBuilderOptions());
        }

        public static async Task<AllocationAmountsFact> Load(WaDEContext db, AllocationAmountsFactBuilderOptions opts)
        {
            opts.OrganizationsDim = opts.OrganizationsDim ?? await OrganizationsDimBuilder.Load(db);
            opts.VariablesDim = opts.VariablesDim ?? await VariablesDimBuilder.Load(db);
            opts.MethodsDim = opts.MethodsDim ?? await MethodsDimBuilder.Load(db);
            opts.DataPublicationDate = opts.DataPublicationDate ?? await DateDimBuilder.Load(db);
            opts.AllocationPriorityDate = opts.AllocationPriorityDate ?? await DateDimBuilder.Load(db);

            var item = Create(opts);

            db.AllocationAmountsFact.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static long GenerateId()
        {
            return (new Faker()).Random.Long(1);
        }
    }

    public class AllocationAmountsFactBuilderOptions
    {
        public OrganizationsDim OrganizationsDim { get; set; }
        public VariablesDim VariablesDim { get; set; }
        public BeneficialUsesCV PrimaryBeneficialUsesCv { get; set; }
        public MethodsDim MethodsDim { get; set; }
        public DateDim TimeframeStart { get; set; }
        public DateDim TimeframeEnd { get; set; }
        public DateDim DataPublicationDate { get; set; }
        public DateDim AllocationApplicationDate { get; set; }
        public DateDim AllocationPriorityDate { get; set; }
        public DateDim AllocationExpirationDate { get; set; }
        public WaterAllocationBasis WaterAllocationBasis { get; set; }
        public LegalStatus LegalStatus { get; set; }
        public WaterAllocationType WaterAllocationType { get; set; }
        public DateDim AllocationTimeframeStartDate { get; set; }
        public DateDim AllocationTimeframeEndDate { get; set; }
        //public ReportYearCv ReportYearCv { get; set; }
        public CropType CropType { get; set; }
        public IrrigationMethod IrrigationMethod { get; set; }
        public CustomerType CustomerType { get; set; }
        public SDWISIdentifier SDWISIdentifier { get; set; }
        public WaterAllocationRecordType? RecordType { get; set; }
    }

    public enum WaterAllocationRecordType
    {
        None,
        Power,
        Civilian,
        Ag
    }
}