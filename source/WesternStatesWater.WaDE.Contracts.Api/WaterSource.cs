namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class WaterSource
    {
        public string WaterSourceName { get; set; }
        public string WaterSourceNativeID { get; set; }
        public string WaterSourceUUID { get; set; }
        public string WaterSourceTypeCV { get; set; }
        public string FreshSalineIndicatorCV { get; set; }
        public object WaterSourceGeometry { get; set; }
    }
}
