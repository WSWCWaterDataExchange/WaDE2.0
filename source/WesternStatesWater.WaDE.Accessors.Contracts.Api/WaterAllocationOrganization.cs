﻿using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Api
{
    public class WaterAllocationOrganization
    {
        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationPurview { get; set; }
        public string OrganizationWebsite { get; set; }
        public string OrganizationPhoneNumber { get; set; }
        public string OrganizationContactName { get; set; }
        public string OrganizationContactEmail { get; set; }
        public string OrganizationState { get; set; }

        public List<WaterSource> WaterSources { get; set; }
        public List<VariableSpecific> VariableSpecifics { get; set; }
        public List<Method> Methods { get; set; }
        public List<BeneficialUse> BeneficialUses { get; set; }
        public List<Allocation> WaterAllocations { get; set; }
        public List<Overlay> RegulatoryOverlays { get; set; }
        public List<Site> Sites { get; set; }
    }
}
