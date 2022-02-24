using NetTopologySuite.Geometries;
using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class SiteAllocationAmountsFilters
    {
        public string SiteUuid { get; set; }
        public string SiteTypeCV { get; set; }
        public string BeneficialUseCv { get; set; }
        public string UsgsCategoryNameCv { get; set; }
        public Geometry Geometry { get; set; }
        public DateTime? StartPriorityDate { get; set; }
        public DateTime? EndPriorityDate { get; set; }
        public DateTime? StartDataPublicationDate { get; set; }
        public DateTime? EndDataPublicationDate { get; set; }
        public string HUC8 { get; set; }
        public string HUC12 { get; set; }
        public string County { get; set; }
        public string State { get; set; }
    }
}
