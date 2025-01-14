using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class SiteFeaturesAreaRequestValidator : AbstractValidator<SiteFeaturesAreaRequest>
{
    public SiteFeaturesAreaRequestValidator()
    {
        Include(new FeaturesSearchRequestBaseValidator());
    }
}