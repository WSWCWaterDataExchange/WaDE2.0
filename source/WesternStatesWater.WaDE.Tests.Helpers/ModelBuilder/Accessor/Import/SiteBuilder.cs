using Bogus;
using System.Collections.Generic;
using System.Linq;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import
{
    public static class SiteBuilder
    {
        public static Site Create(SiteBuilderOptions opts)
        {
            var faker = new Faker<Site>()
                .RuleFor(a => a.SiteUUID, f => opts.Site.SiteUuid)
                .RuleFor(a => a.SiteNativeID, f => opts.Site.SiteNativeId)
                .RuleFor(a => a.SiteName, f => opts.Site.SiteName)
                .RuleFor(a => a.USGSSiteID, f => opts.Site.UsgssiteId)
                .RuleFor(a => a.SiteTypeCV, f => opts.Site.SiteTypeCv)
                .RuleFor(a => a.Longitude, f => opts.Site.Longitude.ToString())
                .RuleFor(a => a.Latitude, f => opts.Site.Latitude.ToString())
                .RuleFor(a => a.CoordinateMethodCV, f => opts.Site.CoordinateMethodCv)
                .RuleFor(a => a.CoordinateAccuracy, f => opts.Site.CoordinateAccuracy)
                .RuleFor(a => a.GNISCodeCV, f => opts.Site.GniscodeCv)
                .RuleFor(a => a.EPSGCodeCV, f => opts.Site.EpsgcodeCv)
                .RuleFor(a => a.HUC8, f => opts.Site.HUC8)
                .RuleFor(a => a.HUC12, f => opts.Site.HUC12)
                .RuleFor(a => a.County, f => opts.Site.County)
                .RuleFor(a => a.PODorPOUSite, f => opts.Site.PODorPOUSite)
                .RuleFor(a => a.OverlayUUIDs, f => opts.RegulatoryOverlayDims == null ? "" : string.Join(',', opts.RegulatoryOverlayDims.Select(x => x.OverlayUuid)))
                .RuleFor(a => a.WaterSourceUUIDs, f => opts.WaterSourceDims == null ? "" : string.Join(',', opts.WaterSourceDims.Select(x => x.WaterSourceUuid)));

            return faker;
        }
    }

    public class SiteBuilderOptions
    {
        public SitesDim Site { get; set; }
        public List<OverlayDim> RegulatoryOverlayDims { get; set; }
        public List<WaterSourcesDim> WaterSourceDims { get; set; }
    }
}