using FluentValidation;
using WesternStatesWater.WaDE.Contracts.Api.Extensions;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class OverlayFeaturesAreaRequestValidator : AbstractValidator<OverlayFeaturesAreaRequest>
{
    public OverlayFeaturesAreaRequestValidator()
    {
        Include(new FeaturesSearchRequestBaseValidator());

        // https://docs.ogc.org/is/19-086r6/19-086r6.html#_b749d106-9940-46cc-9ffe-5128f5bcdcea
        // If an unsupported Well Known Text (WKT) geometry is requested a 400 error SHOULD be returned.
        RuleFor(req => req.Coords)
            .Cascade(CascadeMode.Stop)
            .ValidAreaWkt();
    }
}