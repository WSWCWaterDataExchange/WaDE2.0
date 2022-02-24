using NetTopologySuite.Geometries;
using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class SiteVariableAmountsFilters
    {
        public string SiteTypeCv { get; set; }
        public string VariableCv { get; set; }
        public string VariableSpecificCv { get; set; }
        public string BeneficialUseCv { get; set; }
        public string UsgsCategoryNameCv { get; set; }
        public Geometry Geometry { get; set; }
        public DateTime? TimeframeStartDate { get; set; }
        public DateTime? TimeframeEndDate { get; set; }
        public string SiteUuid { get; set; }
        public string HUC8 { get; set; }
        public string HUC12 { get; set; }
        public string County { get; set; }
        public string State { get; set; }
    }
}
