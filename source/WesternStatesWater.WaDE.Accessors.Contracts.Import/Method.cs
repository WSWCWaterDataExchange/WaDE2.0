using CsvHelper.Configuration.Attributes;

namespace WesternStatesWater.WaDE.Accessors.Contracts.Import
{
    public class Method
    {
        [NullValues("")]
        public string MethodUUID { get; set; }

        [NullValues("")]
        public string MethodName { get; set; }

        [NullValues("")]
        public string MethodDescription { get; set; }

        [NullValues("")]
        public string MethodNEMILink { get; set; }

        [NullValues("")]
        public string ApplicableResourceTypeCV { get; set; }

        [NullValues("")]
        public string MethodTypeCV { get; set; }

        [NullValues("")]
        public string DataCoverageValue { get; set; }

        [NullValues("")]
        public string DataQualityValueCV { get; set; }

        [NullValues("")]
        public string DataConfidenceValue { get; set; }
    }
}
