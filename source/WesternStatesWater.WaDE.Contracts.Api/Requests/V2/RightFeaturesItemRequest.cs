namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class RightFeaturesItemRequest : RightFeaturesSearchRequestBase
{
    /// <summary>
    /// OGC API DateTime parameter used for Priority Date
    /// </summary>
    public string DateTime { get; set; }
}