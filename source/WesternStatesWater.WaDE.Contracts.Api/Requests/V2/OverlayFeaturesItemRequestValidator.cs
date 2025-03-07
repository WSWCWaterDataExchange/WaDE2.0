using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class OverlayFeaturesItemRequestValidator : AbstractValidator<OverlayFeaturesItemRequest>
{
    public OverlayFeaturesItemRequestValidator()
    {
        Include(new FeaturesSearchRequestBaseValidator());
    }
}