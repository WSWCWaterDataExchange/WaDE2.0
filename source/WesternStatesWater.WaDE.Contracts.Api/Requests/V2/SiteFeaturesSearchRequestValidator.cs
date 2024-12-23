using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class SiteFeaturesSearchRequestValidator : AbstractValidator<SiteFeaturesSearchRequest>
{
    public SiteFeaturesSearchRequestValidator()
    {
        When(req => req.Limit.HasValue, () =>
        {
            RuleFor(req => req.Limit)
                .GreaterThan(0)
                .LessThan(10000);
        });
    }
}