namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class WaterSource
    {
        public string WaterSourceUUID { get; set; }
        public string WaterSourceNativeID { get; set; }
        public string WaterSourceName { get; set; }
        public string WaterSourceTypeCV { get; set; }
        public string WaterQualityIndicatorCV { get; set; }
        public string GNISFeatureNameCV { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
