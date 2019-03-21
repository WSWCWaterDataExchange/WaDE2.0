using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WesternStatesWater.WaDE.Accessors.Tests
{
    [TestClass]
    public class DtoMapper
    {
        [TestMethod]
        public void ConfigurationIsValid()
        {
            Mapping.DtoMapper.Configuration.AssertConfigurationIsValid();
        }
    }
}
