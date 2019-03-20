using System;
using System.Collections.Generic;
using GeoAPI.Geometries;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class SitesDim
    {
        public SitesDim()
        {
            AllocationAmountsFact = new HashSet<AllocationAmountsFact>();
            SiteVariableAmountsFact = new HashSet<SiteVariableAmountsFact>();
        }

        public long SiteId { get; set; }
        public string SiteUuid { get; set; }
        public string SiteNativeId { get; set; }
        public string SiteName { get; set; }
        public string SiteTypeCv { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public IGeometry SitePoint { get; set; }
        public IGeometry Geometry { get; set; }
        public string CoordinateMethodCv { get; set; }
        public string CoordinateAccuracy { get; set; }
        public string GniscodeCv { get; set; }
        public long? NhdmetadataId { get; set; }

        public virtual Nhdmetadata Nhdmetadata { get; set; }
        public virtual ICollection<AllocationAmountsFact> AllocationAmountsFact { get; set; }
        public virtual ICollection<SiteVariableAmountsFact> SiteVariableAmountsFact { get; set; }
    }
}
