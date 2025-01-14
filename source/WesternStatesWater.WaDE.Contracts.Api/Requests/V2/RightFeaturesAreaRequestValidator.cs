using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class RightFeaturesAreaRequestValidator :AbstractValidator<RightFeaturesAreaRequest>
{
    public RightFeaturesAreaRequestValidator()
    {
        Include(new FeaturesSearchRequestBaseValidator());
    }
}