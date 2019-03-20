using System;
using System.Collections.Generic;
using GeoAPI.Geometries;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class WaterSourcesDim
    {
        public WaterSourcesDim()
        {
            AggregatedAmountsFact = new HashSet<AggregatedAmountsFact>();
            AllocationAmountsFact = new HashSet<AllocationAmountsFact>();
            SiteVariableAmountsFact = new HashSet<SiteVariableAmountsFact>();
        }

        public long WaterSourceId { get; set; }
        public string WaterSourceUuid { get; set; }
        public string WaterSourceNativeId { get; set; }
        public string WaterSourceName { get; set; }
        public string WaterSourceTypeCv { get; set; }
        public string WaterQualityIndicatorCv { get; set; }
        public string GnisfeatureNameCv { get; set; }
        public IGeometry Geometry { get; set; }

        public virtual ICollection<AggregatedAmountsFact> AggregatedAmountsFact { get; set; }
        public virtual ICollection<AllocationAmountsFact> AllocationAmountsFact { get; set; }
        public virtual ICollection<SiteVariableAmountsFact> SiteVariableAmountsFact { get; set; }
    }
}
