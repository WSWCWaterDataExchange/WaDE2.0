using System;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class SiteDigest
    {
        public string SiteUUID { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
    }
}