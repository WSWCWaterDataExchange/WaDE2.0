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
            SitesAllocationAmountsBridgeFact = new HashSet<SitesAllocationAmountsBridgeFact>();
        }

        public long SiteId { get; set; }
        public string SiteUuid { get; set; }
        public string SiteNativeId { get; set; }
        public string SiteName { get; set; }
        public string UsgssiteId { get; set; }
        public string SiteTypeCv { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public IGeometry SitePoint { get; set; }
        public IGeometry Geometry { get; set; }
        public string CoordinateMethodCv { get; set; }
        public string CoordinateAccuracy { get; set; }
        public string GniscodeCv { get; set; }
        public string EpsgcodeCv { get; set; }
        public long? NhdmetadataId { get; set; }
        public string NhdnetworkStatusCv { get; set; }
        public string NhdproductCv { get; set; }

        public virtual CoordinateMethod CoordinateMethodCvNavigation { get; set; }
        public virtual Epsgcode EpsgcodeCvNavigation { get; set; }
        public virtual GnisfeatureName GniscodeCvNavigation { get; set; }
        public virtual Nhdmetadata Nhdmetadata { get; set; }
        public virtual NhdnetworkStatus NhdnetworkStatusCvNavigation { get; set; }
        public virtual Nhdproduct NhdproductCvNavigation { get; set; }
        public virtual SiteType SiteTypeCvNavigation { get; set; }
        public virtual ICollection<AllocationAmountsFact> AllocationAmountsFact { get; set; }
        public virtual ICollection<SiteVariableAmountsFact> SiteVariableAmountsFact { get; set; }
        public virtual ICollection<SitesAllocationAmountsBridgeFact> SitesAllocationAmountsBridgeFact { get; set; }
    }
}
