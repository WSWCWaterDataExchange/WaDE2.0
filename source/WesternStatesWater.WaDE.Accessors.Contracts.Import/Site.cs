using CsvHelper.Configuration.Attributes;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class Site
    {
        [NullValues("")]
        public string SiteUUID { get; set; }

        [NullValues("")]
        public string SiteNativeID { get; set; }

        [NullValues("")]
        public string SiteName { get; set; }

        [NullValues("")]
        public string USGSSiteID { get; set; }

        [NullValues("")]
        public string SiteTypeCV { get; set; }

        [NullValues("")]
        public string Longitude { get; set; }

        [NullValues("")]
        public string Latitude { get; set; }

        [NullValues("")]
        public string Geometry { get; set; }

        [NullValues("")]
        public string CoordinateMethodCV { get; set; }

        [NullValues("")]
        public string CoordinateAccuracy { get; set; }

        [NullValues("")]
        public string GNISCodeCV { get; set; }

        [NullValues("")]
        public string EPSGCodeCV { get; set; }

        public long? NHDMetadataID { get; set; }
    }
}
