using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests;

public class FeatureItemSearchRequestBaseValidator : AbstractValidator<FeatureItemSearchRequestBase>
{
    public FeatureItemSearchRequestBaseValidator()
    {
        RuleFor(req => req.Id)
            .NotEmpty();
    }
}