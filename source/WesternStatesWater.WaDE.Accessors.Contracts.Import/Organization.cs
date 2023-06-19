using CsvHelper.Configuration.Attributes;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class Organization
    {
        [NullValues("")]
        public string OrganizationUUID { get; set; }

        [NullValues("")]
        public string OrganizationName { get; set; }

        [NullValues("")]
        public string OrganizationPurview { get; set; }

        [NullValues("")]
        public string OrganizationWebsite { get; set; }

        [NullValues("")]
        public string OrganizationPhoneNumber { get; set; }

        [NullValues("")]
        public string OrganizationContactName { get; set; }

        [NullValues("")]
        public string OrganizationContactEmail { get; set; }

        [NullValues("")]
        public string State { get; set; }
    }
}
