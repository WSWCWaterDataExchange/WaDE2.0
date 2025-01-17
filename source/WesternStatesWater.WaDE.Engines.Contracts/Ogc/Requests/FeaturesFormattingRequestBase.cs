namespace WesternStatesWater.WaDE.Engines.Contracts.Ogc.Requests;

public abstract class FeaturesFormattingRequestBase : FormattingRequestBase
{
    public required string CollectionId { get; init; }
    public required FeatureBase[] Items { get; init; }
    public string? LastUuid { get; set; }
}
// SiteFeature
// /collections/overlays/items?siteUuids=abc123
// /collections/rights/items?siteUuids=abc123

// RightFeature
// /collections/overlays/items?rightUuids=abc123
// /collections/sites/items?rightUuids=abc123

// OverlayFeature
// /collections/sites/items?overlayUuids=abc123
// /collections/rights/items?overlayUuids=abc123