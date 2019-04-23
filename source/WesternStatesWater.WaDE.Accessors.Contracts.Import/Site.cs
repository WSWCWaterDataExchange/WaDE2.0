using GeoAPI.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class Site
    {
        public string SiteUUID { get; set; }
        public string SiteNativeID { get; set; }
        public string SiteName { get; set; }
        public string USGSSiteID { get; set; }
        public string SiteTypeCV { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public IGeometry Geometry { get; set; }
        public string CoordinateMethodCV { get; set; }
        public string CoordinateAccuracy { get; set; }
        public string GNISCodeCV { get; set; }
        public string EPSGCodeCV { get; set; }
        public long? NHDMetadataID { get; set; }
    }
}
