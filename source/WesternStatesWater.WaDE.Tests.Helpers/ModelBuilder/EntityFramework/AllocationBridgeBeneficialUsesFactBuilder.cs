using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

public static class AllocationBridgeBeneficialUsesFactBuilder
{
    public static AllocationBridgeBeneficialUsesFact Create()
    {
        return Create(new AllocationBridgeBeneficialUsesFactBuilderOptions());
    }

    public static AllocationBridgeBeneficialUsesFact Create(AllocationBridgeBeneficialUsesFactBuilderOptions opts)
    {
        var faker = new Faker<AllocationBridgeBeneficialUsesFact>()
            .RuleFor(a => a.AllocationAmountId,
                opts?.AllocationAmountsFact?.AllocationAmountId ?? AllocationAmountsFactBuilder.GenerateId())
            .RuleFor(a => a.BeneficialUse, opts?.BeneficialUsesCv ?? BeneficalUsesBuilder.Create())
            .RuleFor(a => a.BeneficialUseCV, (_, fact) => fact.BeneficialUse.WaDEName);

        return faker;
    }
    
    public static async Task<AllocationBridgeBeneficialUsesFact> Load(WaDEContext db)
    {
        return await Load(db, new AllocationBridgeBeneficialUsesFactBuilderOptions());
    }
    
    public static async Task<AllocationBridgeBeneficialUsesFact> Load(WaDEContext db, AllocationBridgeBeneficialUsesFactBuilderOptions opts)
    {
        opts.AllocationAmountsFact ??= await AllocationAmountsFactBuilder.Load(db);
        opts.BeneficialUsesCv ??= await BeneficalUsesBuilder.Load(db);
        
        var item = Create(opts);

        db.AllocationBridgeBeneficialUsesFact.Add(item);
        await db.SaveChangesAsync();

        return item;
    }
}

public class AllocationBridgeBeneficialUsesFactBuilderOptions
{
    public AllocationAmountsFact AllocationAmountsFact { get; set; }
    public BeneficialUsesCV BeneficialUsesCv { get; set; }
}