using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class RightFeaturesItemRequestValidator : AbstractValidator<RightFeaturesItemRequest>
{
    public RightFeaturesItemRequestValidator()
    {
        Include(new FeaturesSearchRequestBaseValidator());
    }
}