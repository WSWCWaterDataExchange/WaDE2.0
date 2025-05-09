﻿using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class OverlayReportingUnitsOrganization
    {
        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationPurview { get; set; }
        public string OrganizationWebsite { get; set; }
        public string OrganizationPhoneNumber { get; set; }
        public string OrganizationContactName { get; set; }
        public string OrganizationContactEmail { get; set; }
        public string OrganizationState { get; set; }

        public List<Overlay> Overlays { get; set; }
        public List<ReportingUnitOverlay> ReportingUnitsOverlay { get; set; }
    }
}
