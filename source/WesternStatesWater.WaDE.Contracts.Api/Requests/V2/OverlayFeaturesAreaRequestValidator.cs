using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class OverlayFeaturesAreaRequestValidator : AbstractValidator<OverlayFeaturesAreaRequest>
{
    public OverlayFeaturesAreaRequestValidator()
    {
        Include(new FeaturesSearchRequestBaseValidator());
    }
}