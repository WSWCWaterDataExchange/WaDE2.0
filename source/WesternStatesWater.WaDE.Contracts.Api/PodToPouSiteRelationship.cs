using System;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class PodToPouSiteRelationship
    {
        public string SiteUUID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}