using System.Threading.Tasks;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api;

public interface ISiteAccessor
{
    public Task<SiteMetadata> GetSiteMetadata();
}