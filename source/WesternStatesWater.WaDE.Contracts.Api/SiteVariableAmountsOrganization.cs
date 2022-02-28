using System.Collections.Generic;

namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class SiteVariableAmountsOrganization
    {
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
        public List<SiteVariableAmount> SiteVariableAmounts { get; set; }
        public List<Site> Sites { get; set; }
    }
}
