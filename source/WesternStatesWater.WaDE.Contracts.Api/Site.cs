using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class Site
    {
        public string NativeSiteID { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string SiteGeometry { get; set; }
        public string CoordinateMethodCV { get; set; }
        public string AllocationGNISIDCV { get; set; }
    }
}