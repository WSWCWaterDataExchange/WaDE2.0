using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class SiteFeaturesItemRequestValidator : AbstractValidator<SiteFeaturesItemRequest>
{
    public SiteFeaturesItemRequestValidator()
    {
        Include(new FeaturesSearchRequestBaseValidator());
    }
}