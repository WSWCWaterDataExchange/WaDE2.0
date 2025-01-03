using System.Linq;
using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class SiteFeaturesSearchRequestValidator : AbstractValidator<SiteFeaturesSearchRequest>
{
    public SiteFeaturesSearchRequestValidator()
    {
        When(req => req.Bbox != null, () =>
        {
            RuleFor(req => req.Bbox)
                .NotEmpty()
                .Must(r =>
                    r.Split(',').Length == 4 &&
                    r.Split(',').All(v => double.TryParse(v, out _)))
                .DependentRules(() =>
                {
                    RuleFor(r => r.Bbox)
                        .Must(r =>
                        {
                            var bbox = r.Split(',').Select(double.Parse).ToArray();
                            if (bbox[0] is < -180 or > 180 || bbox[2] is < -180 or > 180)
                                return false;

                            return bbox[1] is >= -90 and <= 90 && bbox[3] is >= -90 and <= 90;
                        })
                        .WithMessage("Bounding box requires four values: minX, minY, maxX, and maxY, with longitudes between -180 and 180 degrees and latitudes between -90 and 90 degrees.");
                })
                .WithMessage("Bounding box requires four values: minX, minY, maxX, and maxY, with longitudes between -180 and 180 degrees and latitudes between -90 and 90 degrees.");
        });

        When(req => req.Limit != null, () =>
        {
            RuleFor(req => req.Limit)
                .NotEmpty()
                .Must(r => int.TryParse(r, out var val) && val is > 0 and <= 10000)
                .WithMessage("Limit must be a number between 1 and 10000.");
        });
    }
}