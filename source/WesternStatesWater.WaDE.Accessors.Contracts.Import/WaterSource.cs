using CsvHelper.Configuration.Attributes;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class WaterSource
    {
        [NullValues("")]
        public string WaterSourceUUID { get; set; }

        [NullValues("")]
        public string WaterSourceNativeID { get; set; }

        [NullValues("")]
        public string WaterSourceName { get; set; }

        [NullValues("")]
        public string WaterSourceTypeCV { get; set; }

        [NullValues("")]
        public string WaterQualityIndicatorCV { get; set; }

        [NullValues("")]
        public string GNISFeatureNameCV { get; set; }

        [NullValues("")]
        public string Geometry { get; set; }
    }
}
