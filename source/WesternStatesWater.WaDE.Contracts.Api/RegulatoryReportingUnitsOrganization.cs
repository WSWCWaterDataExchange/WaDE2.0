using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class RegulatoryReportingUnitsOrganization
    {
        public string OrganizationName { get; set; }
        public string OrganizationPurview { get; set; }
        public string OrganizationWebsite { get; set; }
        public string OrganizationPhoneNumber { get; set; }
        public string OrganizationContactName { get; set; }
        public string OrganizationContactEmail { get; set; }
        public string OrganizationState { get; set; }

        public List<RegulatoryOverlay> RegulatoryOverlays { get; set; }
        public List<ReportingUnitRegulatory> ReportingUnitsRegulatory { get; set; }
    }
}
