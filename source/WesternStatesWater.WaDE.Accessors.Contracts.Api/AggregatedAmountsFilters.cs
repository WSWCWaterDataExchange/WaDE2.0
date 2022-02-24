using System;
using NetTopologySuite.Geometries;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class AggregatedAmountsFilters
    {
        public string VariableCV { get; set; }
        public string VariableSpecificCV { get; set; }
        public string BeneficialUse { get; set; }
        public string ReportingUnitUUID { get; set; }
        public Geometry Geometry { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ReportingUnitTypeCV { get; set; }
        public string UsgsCategoryNameCV { get; set; }
        public string State { get; set; }
    }
}
