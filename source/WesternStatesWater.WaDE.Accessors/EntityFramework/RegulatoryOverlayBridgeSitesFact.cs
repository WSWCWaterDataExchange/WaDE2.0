namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class RegulatoryOverlayBridgeSitesFact
    {
        public long RegulatoryOverlayBridgeId { get; set; }
        public long RegulatoryOverlayId { get; set; }
        public long SiteId { get; set; }
        
        public virtual OverlayDim RegulatoryOverlay { get; set; }
        public virtual SitesDim Site { get; set; }
    }
}