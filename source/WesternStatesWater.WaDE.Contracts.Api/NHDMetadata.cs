using System;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class NHDMetadata
    {
        public string NHDNetworkStatusCV { get; set; }
        public string NHDProductCV { get; set; }
        public DateTime NHDUpdateDate { get; set; }
        public string NHDReachCode { get; set; }
        public string NHDMeasureNumber { get; set; }
    }
}
