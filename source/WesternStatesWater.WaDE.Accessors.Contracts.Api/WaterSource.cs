namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class WaterSource
    {
        public string WaterSourceName { get; set; }
        public string WaterSourceCode { get; set; }
        public string WaterSourceTypeCV { get; set; }
        public string FreshSalineIndicatorCV { get; set; }
        //public string Geometry { get; set; }
        public Organization Organization { get; set; }
    }
}
