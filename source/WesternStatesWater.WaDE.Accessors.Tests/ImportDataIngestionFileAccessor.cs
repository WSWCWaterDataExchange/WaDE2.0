using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Tests.Helpers;

namespace WesternStatesWater.WaDE.Accessors.Tests
{
    [TestClass]
    public class ImportDataIngestionFileAccessor
    {
        private readonly IFileStreamFactory FileStreamFactoryMock = Mock.Create<IFileStreamFactory>(Behavior.Strict);
        [TestMethod]
        public async Task GetAggregatedAmounts_IrrigatedAcreageHasHeaderButEmpty()
        {
            var runId = (new Faker()).Random.AlphaNumeric(5);
            FileStreamFactoryMock.Arrange(a => a.GetStreamAsync(Arg.IsAny<IConfiguration>(), runId, "aggregatedamounts.csv"))
                .Returns(Task.FromResult((Stream)new MemoryStream(System.Text.Encoding.UTF8.GetBytes(ImportData.ImportData.GetWaterAllocations_IrrigatedAcrageSpecifiedButEmpty))));

            var sut = CreateWaterAllocationFileAccessor();
            var results = await sut.GetAggregatedAmounts(runId, 0, 10000);

            results.Should().HaveCount(1);
            results[0].IrrigatedAcreage.Should().BeNull();
        }

        [TestMethod]
        public async Task GetSites_BasicDataRead()
        {
            var runId = (new Faker()).Random.AlphaNumeric(5);
            FileStreamFactoryMock.Arrange(a => a.GetStreamAsync(Arg.IsAny<IConfiguration>(), runId, "sites.csv"))
                .Returns(Task.FromResult((Stream)new MemoryStream(System.Text.Encoding.UTF8.GetBytes(ImportData.ImportData.Sites_OneRecord))));

            var sut = CreateWaterAllocationFileAccessor();
            var results = await sut.GetSites(runId, 0, 10000);

            results.Should().HaveCount(1);
            results[0].SiteUUID.Should().Be("NEwr_S1");
            results[0].RegulatoryOverlayUUIDs.Should().Be("abcd");
            results[0].WaterSourceUUIDs.Should().Be("NEwr_WS1,NEwr_WS2");
            results[0].CoordinateAccuracy.Should().Be("1.01");
            results[0].CoordinateMethodCV.Should().Be("Unspecified");
            results[0].County.Should().Be("Thayer");
            results[0].EPSGCodeCV.Should().Be("4326");
            results[0].Geometry.Should().Be("Fake Geometry");
            results[0].GNISCodeCV.Should().Be("999");
            results[0].HUC12.Should().Be("103000000000");
            results[0].HUC8.Should().Be("158400000000");
            results[0].Latitude.Should().Be("40.01104072");
            results[0].Longitude.Should().Be("-97.5223804");
            results[0].PODorPOUSite.Should().Be("POD");
            results[0].SiteName.Should().Be("Unspecified");
            results[0].SiteNativeID.Should().Be("10443");
            results[0].SiteTypeCV.Should().Be("Unspecified");
            results[0].USGSSiteID.Should().Be("Fake USGS");
        }

        [TestMethod]
        public async Task GetOrganizations_BasicDataRead()
        {
            var runId = (new Faker()).Random.AlphaNumeric(5);
            FileStreamFactoryMock.Arrange(a => a.GetStreamAsync(Arg.IsAny<IConfiguration>(), runId, "organizations.csv"))
                .Returns(Task.FromResult((Stream)new MemoryStream(System.Text.Encoding.UTF8.GetBytes(ImportData.ImportData.Organizations_OneRecord))));

            var sut = CreateWaterAllocationFileAccessor();
            var results = await sut.GetOrganizations(runId, 0, 10000);

            results.Should().HaveCount(1);
            results[0].OrganizationUUID.Should().Be("abcd");
            results[0].OrganizationContactEmail.Should().Be("test@fake.gov");
            results[0].OrganizationContactName.Should().Be("Test Contact");
            results[0].OrganizationName.Should().Be("Test Org Name");
            results[0].OrganizationPhoneNumber.Should().Be("402-555-1234");
            results[0].OrganizationPurview.Should().Be("Test Purview, it is awesome");
            results[0].OrganizationWebsite.Should().Be("http://fake.gov/");
            results[0].State.Should().Be("NE");
        }

        [TestMethod]
        public async Task GetMethods_BasicDataRead()
        {
            var runId = (new Faker()).Random.AlphaNumeric(5);
            FileStreamFactoryMock.Arrange(a => a.GetStreamAsync(Arg.IsAny<IConfiguration>(), runId, "methods.csv"))
                .Returns(Task.FromResult((Stream)new MemoryStream(System.Text.Encoding.UTF8.GetBytes(ImportData.ImportData.Methods_OneRecord))));

            var sut = CreateWaterAllocationFileAccessor();
            var results = await sut.GetMethods(runId, 0, 10000);

            results.Should().HaveCount(1);
            results[0].MethodUUID.Should().Be("abcd");
            results[0].ApplicableResourceTypeCV.Should().Be("Groundwater");
            results[0].MethodDescription.Should().Be("Test Water Rights");
            results[0].MethodName.Should().Be("Fake Water Rights");
            results[0].MethodNEMILink.Should().Be("http://fake.water.gov/");
            results[0].MethodTypeCV.Should().Be("Adjudicated");
            results[0].WaDEDataMappingUrl.Should().Be("http://fake.wade.gov/");
        }

        public IDataIngestionFileAccessor CreateWaterAllocationFileAccessor()
        {
            return new DataIngestionFileAccessor(Configuration.GetConfiguration(), null)
            {
                FileStreamFactory = FileStreamFactoryMock
            };
        }
    }
}
