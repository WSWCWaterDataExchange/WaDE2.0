using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class WaterSource
    {
        public long WaterSourceId { get; set; }
        public string WaterSourceName { get; set; }
        public string WaterSourceNativeID { get; set; }
        public string WaterSourceUUID { get; set; }
        public string WaterSourceTypeCV { get; set; }
        public string FreshSalineIndicatorCV { get; set; }
        public Geometry WaterSourceGeometry { get; set; }
    }
}
