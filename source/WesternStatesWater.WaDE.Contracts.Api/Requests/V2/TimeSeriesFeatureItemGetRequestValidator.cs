using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class TimeSeriesFeatureItemGetRequestValidator : AbstractValidator<TimeSeriesFeatureItemGetRequest>
{
    public TimeSeriesFeatureItemGetRequestValidator()
    {
        Include(new FeatureItemSearchRequestBaseValidator());
    }
}