using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class SiteFeatureItemGetRequestValidator : AbstractValidator<SiteFeatureItemGetRequest>
{
    public SiteFeatureItemGetRequestValidator()
    {
        Include(new FeatureItemSearchRequestBaseValidator());
    }
}