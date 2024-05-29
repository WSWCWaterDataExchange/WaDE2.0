using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class Site
    {
        public long SiteID { get; set; }
        public string SiteUUID { get; set; }
        public string SiteName { get; set; }
        public string SiteTypeCV { get; set; }
        public string NativeSiteID { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public Geometry SiteGeometry { get; set; }
        public string CoordinateMethodCV { get; set; }
        public string AllocationGNISIDCV { get; set; }
        public string HUC8 { get; set; }
        public string HUC12 { get; set; }
        public string County { get; set; }
        public string PODorPOUSite { get; set; }
        public string WellDepth { get; set; }
        public List<PodToPouSiteRelationship> RelatedPODSites { get; set; }
        public List<PodToPouSiteRelationship> RelatedPOUSites { get; set; }
        public List<string> WaterSourceUUIDs { get; set; }
    }
}