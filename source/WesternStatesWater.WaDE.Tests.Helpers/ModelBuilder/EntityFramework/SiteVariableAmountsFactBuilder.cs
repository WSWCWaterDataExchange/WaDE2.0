using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class SiteVariableAmountsFactBuilder
    {
        public static SiteVariableAmountsFact Create()
        {
            return Create(new SiteVariableAmountsFactBuilderOptions());
        }

        public static SiteVariableAmountsFact Create(SiteVariableAmountsFactBuilderOptions opts)
        {
            var faker = new Faker<SiteVariableAmountsFact>()
                    .RuleFor(a => a.OrganizationId, f => opts.OrganizationsDim?.OrganizationId ?? OrganizationsDimBuilder.GenerateId())
                    .RuleFor(a => a.SiteId, f => opts.SiteDim?.SiteId ?? SitesDimBuilder.GenerateId())
                    .RuleFor(a => a.VariableSpecificId, f => opts.VariablesDim?.VariableSpecificId ?? VariablesDimBuilder.GenerateId())
                    .RuleFor(a => a.WaterSourceId, f => opts.WaterSourcesDim?.WaterSourceId ?? WaterSourcesDimBuilder.GenerateId())
                    .RuleFor(a => a.MethodId, f => opts.MethodsDim?.MethodId ?? MethodsDimBuilder.GenerateId())
                    .RuleFor(a => a.TimeframeStartID, f => opts.TimeframeStart?.DateId ?? DateDimBuilder.GenerateId())
                    .RuleFor(a => a.TimeframeEndID, f => opts.TimeframeEnd?.DateId ?? DateDimBuilder.GenerateId())
                    .RuleFor(a => a.DataPublicationDateID, f => opts.DataPublicationDate?.DateId ?? DateDimBuilder.GenerateId())
                    .RuleFor(a => a.DataPublicationDoi, f => f.Random.Words(3))
                    .RuleFor(a => a.ReportYearCv, f => opts.ReportYearCv?.Name)
                    .RuleFor(a => a.Amount, f => opts.Amount ?? f.Random.Double(1, 10000))
                    .RuleFor(a => a.PrimaryUseCategoryCV, f => opts.PrimaryBeneficialUsesCv?.Name)
                ;

            switch (opts.RecordType ?? (new Faker()).PickRandom<SiteVariableAmountsRecordType>())
            {
                case SiteVariableAmountsRecordType.Ag:
                    faker.RuleFor(a => a.IrrigatedAcreage, f => f.Random.Double(0, 10000))
                        .RuleFor(a => a.CropTypeCv, f => opts.CropType?.Name)
                        .RuleFor(a => a.IrrigationMethodCv, f => opts.IrrigationMethod?.Name)
                        .RuleFor(a => a.AllocationCropDutyAmount, f => f.Random.Double(0, 1000));
                    break;
                case SiteVariableAmountsRecordType.Power:
                    faker.RuleFor(a => a.PowerGeneratedGwh, f => f.Random.Double(0, 10000));
                    break;
                case SiteVariableAmountsRecordType.Civilian:
                    faker.RuleFor(a => a.PopulationServed, f => f.Random.Long(0, 10000000))
                        .RuleFor(a => a.CommunityWaterSupplySystem, f => f.Address.City())
                        .RuleFor(a => a.CustomerTypeCv, f => opts.CustomerType?.Name)
                        .RuleFor(a => a.SDWISIdentifierCv, f => opts.SDWISIdentifier?.Name);
                    break;
            }

            return faker;
        }

        public static async Task<SiteVariableAmountsFact> Load(WaDEContext db)
        {
            return await Load(db, new SiteVariableAmountsFactBuilderOptions());
        }

        public static async Task<SiteVariableAmountsFact> Load(WaDEContext db, SiteVariableAmountsFactBuilderOptions opts)
        {
            opts.OrganizationsDim = opts.OrganizationsDim ?? await OrganizationsDimBuilder.Load(db);
            opts.SiteDim = opts.SiteDim ?? await SitesDimBuilder.Load(db);
            opts.VariablesDim = opts.VariablesDim ?? await VariablesDimBuilder.Load(db);
            opts.WaterSourcesDim = opts.WaterSourcesDim ?? await WaterSourcesDimBuilder.Load(db);
            opts.MethodsDim = opts.MethodsDim ?? await MethodsDimBuilder.Load(db);
            opts.TimeframeStart = opts.TimeframeStart ?? await DateDimBuilder.Load(db);
            opts.TimeframeEnd = opts.TimeframeEnd ?? await DateDimBuilder.Load(db);
            opts.DataPublicationDate = opts.DataPublicationDate ?? await DateDimBuilder.Load(db);

            var item = Create(opts);

            db.SiteVariableAmountsFact.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static long GenerateId()
        {
            return (new Faker()).Random.Long(1);
        }
    }

    public class SiteVariableAmountsFactBuilderOptions
    {
        public OrganizationsDim OrganizationsDim { get; set; }
        public SitesDim SiteDim { get; set; }
        public VariablesDim VariablesDim { get; set; }
        public BeneficialUsesCV PrimaryBeneficialUsesCv { get; set; }
        public WaterSourcesDim WaterSourcesDim { get; set; }
        public MethodsDim MethodsDim { get; set; }
        public DateDim TimeframeStart { get; set; }
        public DateDim TimeframeEnd { get; set; }
        public DateDim DataPublicationDate { get; set; }
        public CropType CropType { get; set; }
        public IrrigationMethod IrrigationMethod { get; set; }
        public CustomerType CustomerType { get; set; }
        public SDWISIdentifier SDWISIdentifier { get; set; }
        public SiteVariableAmountsRecordType? RecordType { get; set; }
        public ReportYearCv ReportYearCv { get; set; }
        public double? Amount { get; set; }
    }

    public enum SiteVariableAmountsRecordType
    {
        None,
        Power,
        Civilian,
        Ag
    }
}