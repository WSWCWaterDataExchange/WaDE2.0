using System;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class Organization
    {
        public string OrganizationUUID { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationPurview { get; set; }
        public string OrganizationWebsite { get; set; }
        public string OrganizationPhoneNumber { get; set; }
        public string OrganizationContactName { get; set; }
        public string OrganizationContactEmail { get; set; }
        public string OrganizationState { get; set; }
    }

    public class OrganizationRecords
    {
        public int RecordNumber { get; set; }
        public string OrganizationUUID { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationPurview { get; set; }
        public string OrganizationWebsite { get; set; }
        public string OrganizationPhoneNumber { get; set; }
        public string OrganizationContactName { get; set; }
        public string OrganizationContactEmail { get; set; }
        public string OrganizationState { get; set; }
    }
}
