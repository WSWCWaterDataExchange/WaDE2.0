namespace WesternStatesWater.WaDE.Contracts.Api
{
    public class Method
    {
        public string MethodUUID { get; set; }
        public string MethodName { get; set; }
        public string MethodDescription { get; set; }
        public string MethodNEMILink { get; set; }
        public string ApplicableResourceType { get; set; }
        public string MethodTypeCV { get; set; }
        public string DataCoverageValue { get; set; }
        public string DataQualityValue { get; set; }
        public string DataConfidenceValue { get; set; }
    }
}
