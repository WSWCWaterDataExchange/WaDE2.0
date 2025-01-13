using FluentValidation;
using WesternStatesWater.WaDE.Contracts.Api.Extensions;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests;

public class FeaturesSearchRequestBaseValidator : AbstractValidator<FeaturesSearchRequestBase>
{
    public FeaturesSearchRequestBaseValidator()
    {
        When(req => req.Bbox != null, () =>
        {
            RuleFor(req => req.Bbox)
                .Bbox()
                .DependentRules(() => RuleFor(req => req.Bbox).BboxInRange());
        });

        When(req => req.Limit != null, () =>
        {
            RuleFor(req => req.Limit).GreaterThan(0);
        });
    }
}