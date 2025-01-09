using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class OverlayFeaturesSearchRequestValidator : AbstractValidator<OverlayFeaturesSearchRequest>
{
    public OverlayFeaturesSearchRequestValidator()
    {
        Include(new FeaturesSearchRequestBaseValidator());
    }
}