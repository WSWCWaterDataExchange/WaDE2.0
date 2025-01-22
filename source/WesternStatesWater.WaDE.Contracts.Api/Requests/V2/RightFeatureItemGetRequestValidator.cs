using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class RightFeatureItemGetRequestValidator : AbstractValidator<RightFeatureItemGetRequest>
{
    public RightFeatureItemGetRequestValidator()
    {
        Include(new FeatureItemSearchRequestBaseValidator());
    }
}