using System;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class PODToPOUSiteRelationship
    {
        public string PODSiteUUID { get; set; }
        public string POUSiteUUID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}