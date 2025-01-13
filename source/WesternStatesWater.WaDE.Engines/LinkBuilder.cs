using WesternStatesWater.WaDE.Contracts.Api.OgcApi;
using Link = WesternStatesWater.WaDE.Engines.Contracts.Ogc.Link;

namespace WesternStatesWater.WaDE.Engines;

internal class LinkBuilder(string baseUrl, string apiPath)
{
    private readonly List<Link> _links = new();
    private readonly string _baseUrl = TrimEnding(baseUrl);
    private readonly string _apiPath = TrimEnding(apiPath);

    public LinkBuilder AddLandingPage()
    {
        AddLink("/swagger/ui", "text/html", "root", "Landing page");
        AddLink("/swagger.json", "application/json", "root", "Landing page");
        return this;
    }

    public LinkBuilder AddCollections()
    {
        return AddLink($"{_apiPath}/collections", "application/json", "self", "This document as JSON");
    }

    public LinkBuilder AddCollection(string collectionId)
    {
        AddLink($"{_apiPath}/collections/{collectionId}", "application/json", "self");
        AddLink($"{_apiPath}/collections/{collectionId}/items", "application/geo+json", "items");
        return this;
    }
    
    public LinkBuilder AddNextFeatures(string collectionId, string lastUuid)
    {
        AddLink($"{_apiPath}/collections/{collectionId}/items?next={lastUuid}", "application/geo+json", "next");
        return this;
    }

    public Link[] Build()
    {
        return _links.ToArray();
    }

    private LinkBuilder AddLink(string path, string type, string rel, string? title = null)
    {
        _links.Add(new Link
        {
            Href = $"{_baseUrl}/{TrimStart(path)}",
            Type = type,
            Rel = rel,
            Title = title
        });
        return this;
    }

    private static string TrimEnding(string path)
    {
        return path.TrimEnd('/');
    }

    private static string TrimStart(string path)
    {
        return path.TrimStart('/');
    }
}