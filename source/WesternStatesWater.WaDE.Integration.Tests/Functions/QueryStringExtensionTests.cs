using System.Collections.Specialized;
using FluentAssertions;
using WaDEApiFunctions.Extensions;
using WesternStatesWater.Shared.DataContracts;

namespace WesternStatesWater.WaDE.Integration.Tests.Functions;

[TestClass]
public class QueryStringExtensionTests
{
    [TestMethod]
    public void ContainsUnmatchedQueryParameters()
    {
        // Arrange
        var query = new NameValueCollection();
        query.Add(nameof(TestRequest.TestIds).ToLower(), "1,2,3"); // Matched, case insensitive
        query.Add("dummy", "dummy"); // Unmatched

        // Act
        var (result, unmatchedParams) = query.ContainsUnmatchedParameters<TestRequest>();

        // Assert
        result.Should().BeTrue();
        unmatchedParams.Should().Contain("dummy");
    }
}

public class TestRequest : RequestBase
{
    public string TestIds { get; set; }
}