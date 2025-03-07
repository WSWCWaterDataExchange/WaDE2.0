using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class OverlayFeatureItemGetRequestValidator : AbstractValidator<OverlayFeatureItemGetRequest>
{
    public OverlayFeatureItemGetRequestValidator()
    {
        Include(new FeatureItemSearchRequestBaseValidator());
    }
}