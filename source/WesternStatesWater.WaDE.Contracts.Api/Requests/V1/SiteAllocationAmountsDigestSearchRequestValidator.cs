using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V1;

public class SiteAllocationAmountsDigestSearchRequestValidator
    : AbstractValidator<SiteAllocationAmountsDigestSearchRequest>
{
    public SiteAllocationAmountsDigestSearchRequestValidator()
    {
        // This class is necessary to use our newer patterns but is empty for legacy
        // reasons (the validation is done in the client).
    }
}