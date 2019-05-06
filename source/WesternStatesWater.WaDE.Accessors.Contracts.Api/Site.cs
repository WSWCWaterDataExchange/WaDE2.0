namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class Site
    {
        public string SiteCode { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        //public string Geometry { get; set; }
        public string VerticalDatumEPSGCodeCV { get; set; }
        public string CoordinateMethod { get; set; }
        public string GNISCodeCV { get; set; }
        public string EPSGCodeCV { get; set; }
        public string State { get; set; }
        public double? AllocationAcreage { get; set; }
        public string AllocationBasisCV { get; set; }
        public NHDMetadata NHDMetadata { get; set; }
    }
}
