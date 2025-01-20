using System.Linq;
using FluentValidation;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class CollectionMetadataGetRequestValidator : AbstractValidator<CollectionMetadataGetRequest>
{
    public CollectionMetadataGetRequestValidator()
    {
        RuleFor(x => x.RequestUri)
            .NotEmpty()
            .Must(req => req.Segments.Contains("collections/"));
    }
}