using System;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class SiteAllocationAmountsDigestFilters
    {
        public DateTime? StartPriorityDate { get; set; }
        public DateTime? EndPriorityDate { get; set; }
        public string OrganizationUUID { get; set; }
        public string SiteTypeCV { get; set; }
        public string UsgsCategoryNameCv { get; set; }
        public string BeneficialUseCv { get; set; }
        public Geometry Geometry { get; set; }
    }
}
