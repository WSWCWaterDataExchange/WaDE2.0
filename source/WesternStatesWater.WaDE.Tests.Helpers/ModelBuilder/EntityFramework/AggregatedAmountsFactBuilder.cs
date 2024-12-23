using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class AggregatedAmountsFactBuilder
    {
        public static AggregatedAmountsFact Create()
        {
            return Create(new AggregatedAmountsFactBuilderOptions());
        }

        public static AggregatedAmountsFact Create(AggregatedAmountsFactBuilderOptions opts)
        {
            var faker = new Faker<AggregatedAmountsFact>()
                .RuleFor(a => a.OrganizationId, f => opts.OrganizationsDim?.OrganizationId ?? OrganizationsDimBuilder.GenerateId())
                .RuleFor(a => a.ReportingUnitId, f => opts.ReportingUnitsDim?.ReportingUnitId ?? ReportingUnitsDimBuilder.GenerateId())
                .RuleFor(a => a.VariableSpecificId, f => opts.VariablesDim?.VariableSpecificId ?? VariablesDimBuilder.GenerateId())
                .RuleFor(a => a.PrimaryUseCategoryCV, f => opts.PrimaryBeneficialUsesCv?.Name)
                .RuleFor(a => a.WaterSourceId, f => opts.WaterSourcesDim?.WaterSourceId ?? WaterSourcesDimBuilder.GenerateId())
                .RuleFor(a => a.MethodId, f => opts.MethodsDim?.MethodId ?? MethodsDimBuilder.GenerateId())
                .RuleFor(a => a.TimeframeStartId, f => opts.TimeframeStart?.DateId)
                .RuleFor(a => a.TimeframeEndId, f => opts.TimeframeEnd?.DateId)
                .RuleFor(a => a.DataPublicationDateID, f => opts.DataPublicationDate?.DateId)
                .RuleFor(a => a.DataPublicationDoi, f => f.Random.Word())
                .RuleFor(a => a.ReportYearCv, f => opts.ReportYearCv?.Name)
                .RuleFor(a => a.Amount, f => f.Random.Double(0, 5000))
                .RuleFor(a => a.InterbasinTransferToId, f => f.Random.AlphaNumeric(20))
                .RuleFor(a => a.InterbasinTransferFromId, f => f.Random.AlphaNumeric(20));

            switch (opts.RecordType ?? (new Faker()).PickRandom<AggregatedAmountsRecordType>())
            {
                case AggregatedAmountsRecordType.Ag:
                    faker.RuleFor(a => a.IrrigatedAcreage, f => f.Random.Double(0, 10000))
                        .RuleFor(a => a.CropTypeCV, f => opts.CropType?.Name)
                        .RuleFor(a => a.IrrigationMethodCV, f => opts.IrrigationMethod?.Name)
                        .RuleFor(a => a.AllocationCropDutyAmount, f => f.Random.Double(0, 50000));
                    break;
                case AggregatedAmountsRecordType.Power:
                    faker.RuleFor(a => a.PowerGeneratedGwh, f => f.Random.Double(0, 10000));
                    break;
                case AggregatedAmountsRecordType.Civilian:
                    faker.RuleFor(a => a.PopulationServed, f => f.Random.Long(0, 10000000))
                        .RuleFor(a => a.CommunityWaterSupplySystem, f => f.Address.City())
                        .RuleFor(a => a.CustomerTypeCV, f => opts.CustomerType?.Name)
                        .RuleFor(a => a.SDWISIdentifierCV, f => opts.SDWISIdentifier?.Name);
                    break;
            }

            return faker;
        }

        public static async Task<AggregatedAmountsFact> Load(WaDEContext db)
        {
            return await Load(db, new AggregatedAmountsFactBuilderOptions());
        }

        public static async Task<AggregatedAmountsFact> Load(WaDEContext db, AggregatedAmountsFactBuilderOptions opts)
        {
            opts.OrganizationsDim = opts.OrganizationsDim ?? await OrganizationsDimBuilder.Load(db);
            opts.ReportingUnitsDim = opts.ReportingUnitsDim ?? await ReportingUnitsDimBuilder.Load(db);
            opts.VariablesDim = opts.VariablesDim ?? await VariablesDimBuilder.Load(db);
            opts.WaterSourcesDim = opts.WaterSourcesDim ?? await WaterSourcesDimBuilder.Load(db);
            opts.MethodsDim = opts.MethodsDim ?? await MethodsDimBuilder.Load(db);

            var item = Create(opts);

            await db.AggregatedAmountsFact.AddAsync(item);
            await db.SaveChangesAsync();

            return item;
        }
    }

    public class AggregatedAmountsFactBuilderOptions
    {
        public OrganizationsDim OrganizationsDim { get; set; }
        public ReportingUnitsDim ReportingUnitsDim { get; set; }
        public VariablesDim VariablesDim { get; set; }
        public BeneficialUsesCV PrimaryBeneficialUsesCv { get; set; }
        public WaterSourcesDim WaterSourcesDim { get; set; }
        public MethodsDim MethodsDim { get; set; }
        public DateDim TimeframeStart { get; set; }
        public DateDim TimeframeEnd { get; set; }
        public DateDim DataPublicationDate { get; set; }
        public ReportYearCv ReportYearCv { get; set; }
        public CropType CropType { get; set; }
        public IrrigationMethod IrrigationMethod { get; set; }
        public CustomerType CustomerType { get; set; }
        public SDWISIdentifier SDWISIdentifier { get; set; }

        public AggregatedAmountsRecordType? RecordType { get; set; }
    }

    public enum AggregatedAmountsRecordType
    {
        None,
        Power,
        Civilian,
        Ag
    }
}