using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class PodToPouSiteRelationship
    {
        public string PODSiteUUID { get; set; }     
        public string POUSiteUUID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get;  set; }
    }
}