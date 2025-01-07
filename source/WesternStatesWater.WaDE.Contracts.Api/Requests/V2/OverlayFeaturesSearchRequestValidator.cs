using FluentValidation;
using WesternStatesWater.WaDE.Contracts.Api.Extensions;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class OverlayFeaturesSearchRequestValidator : AbstractValidator<OverlayFeaturesSearchRequest>
{
    public OverlayFeaturesSearchRequestValidator()
    {
        When(req => req.Bbox != null, () =>
        {
            RuleFor(req => req.Bbox)
                .Bbox()
                .DependentRules(() => RuleFor(req => req.Bbox).BboxInRange());
        });

        When(req => req.Limit != null, () =>
        {
            RuleFor(req => req.Limit).LimitInRange(1, 1000);
        });
    }
}