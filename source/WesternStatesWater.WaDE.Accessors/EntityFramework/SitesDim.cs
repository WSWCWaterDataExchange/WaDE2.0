using GeoAPI.Geometries;
using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.EntityFramework
{
    public partial class SitesDim
    {
        public SitesDim()
        {
            AllocationBridgeSitesFact = new HashSet<AllocationBridgeSitesFact>();
            SiteVariableAmountsFact = new HashSet<SiteVariableAmountsFact>();
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
        public string NhdnetworkStatusCv { get; set; }
        public string NhdproductCv { get; set; }
        public string StateCv { get; set; }
        public string HUC8 { get; set; }
        public string HUC12 { get; set; }
        public string County { get; set; }

        public virtual CoordinateMethod CoordinateMethodCvNavigation { get; set; }
        public virtual Epsgcode EpsgcodeCvNavigation { get; set; }
        public virtual GnisfeatureName GniscodeCvNavigation { get; set; }
        public virtual NhdnetworkStatus NhdnetworkStatusCvNavigation { get; set; }
        public virtual Nhdproduct NhdproductCvNavigation { get; set; }
        public virtual SiteType SiteTypeCvNavigation { get; set; }
        public virtual State StateCVNavigation { get; set; }
        public virtual ICollection<AllocationBridgeSitesFact> AllocationBridgeSitesFact { get; set;  }
        public virtual ICollection<SiteVariableAmountsFact> SiteVariableAmountsFact { get; set; }
    }
}
