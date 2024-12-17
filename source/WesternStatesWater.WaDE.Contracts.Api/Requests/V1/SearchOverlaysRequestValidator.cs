using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V1;

public class SearchOverlaysRequestValidator : AbstractValidator<SearchOverlaysRequest>
{
    public SearchOverlaysRequestValidator()
    {
        RuleFor(x => x).NotNull();
    }
}