using System;

namespace WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

public class CollectionMetadataGetRequest : MetadataLoadRequestBase
{
    public Uri RequestUri { get; set; }
}