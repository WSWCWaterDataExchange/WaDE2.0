using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class SiteFeaturesSearchRequestValidator : AbstractValidator<SiteFeaturesSearchRequest>
{
    public SiteFeaturesSearchRequestValidator()
    {
        Include(new FeaturesSearchRequestBaseValidator());
    }
}