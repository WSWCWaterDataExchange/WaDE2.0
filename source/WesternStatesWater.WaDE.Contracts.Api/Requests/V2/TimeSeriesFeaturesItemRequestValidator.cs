using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class TimeSeriesFeaturesItemRequestValidator : AbstractValidator<TimeSeriesFeaturesItemRequest>
{
    public TimeSeriesFeaturesItemRequestValidator()
    {
        Include(new FeaturesSearchRequestBaseValidator());
    }
}