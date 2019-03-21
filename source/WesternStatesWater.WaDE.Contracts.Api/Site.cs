namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class Site
    {
        public string SiteCode { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        //public string Geometry { get; set; }
        public string VerticalDatumEPSGCodeCV { get; set; }
        public string CoordinateMethod { get; set; }
        public string GNISCodeCV { get; set; }
        public string State { get; set; }
        public double? AllocationAcreage { get; set; }
        public string AllocationBasisCV { get; set; }
        public NHDMetadata NHDMetadata { get; set; }
        public Organization Organization { get; set; }
    }
}
