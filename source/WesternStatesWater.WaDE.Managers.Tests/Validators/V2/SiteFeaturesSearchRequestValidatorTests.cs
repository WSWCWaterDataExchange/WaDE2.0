using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

namespace WesternStatesWater.WaDE.Managers.Tests.Validators.V2;

[TestClass]
public class SiteFeaturesSearchRequestValidatorTests
{
    [DataTestMethod]
    [DataRow(null)]
    [DataRow("1,2,3,4")]
    [DataRow("1,  2,   3,    4")]
    [DataRow("-1, 2.0, -180, 90")]
    public void Bbox_IsValid_ReturnsTrue(string input)
    {
        // Arrange
        var validator = new SiteFeaturesSearchRequestValidator();
        var request = new SiteFeaturesSearchRequest { Bbox = input };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("1,2,3")]
    [DataRow("1,2,3,")]
    [DataRow("1,2,3,4,5")]
    [DataRow("dog,cat,pig,cow")]
    [DataRow("1.1.1, 2.2.2, 3.3.3, 4.4.4")]
    [DataRow("-181, 0, 0 ,0")]
    [DataRow("181, 0 , 0, 0")]
    [DataRow("0, -91, 0 ,0")]
    [DataRow("0, 91 , 0, 0")]
    [DataRow("0, 0, -181 ,0")]
    [DataRow("0, 0, 181, 0")]
    [DataRow("0, 0, 0 ,-91")]
    [DataRow("0, 0, 0, 91")]
    public void Bbox_Invalid_ReturnsFalseAndContainsError(string input)
    {
        // Arrange
        var validator = new SiteFeaturesSearchRequestValidator();
        var request = new SiteFeaturesSearchRequest { Bbox = input };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Bbox");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Bounding box requires four values: minX, minY, maxX, and maxY, with longitudes between -180 and 180 degrees and latitudes between -90 and 90 degrees.For example, \"-114.052,36.997,-109.041,42.001\"");
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("1")]
    [DataRow("10000")]
    public void Next_IsValid_ReturnsTrue(string input)
    {
        // Arrange
        var validator = new SiteFeaturesSearchRequestValidator();
        var request = new SiteFeaturesSearchRequest { Next = input };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [DataTestMethod]
    [DataRow("0")]
    [DataRow("-1")]
    [DataRow("10.1")]
    [DataRow("10001")]
    [DataRow("dog")]
    public void Limit_Invalid_ReturnsFalseAndContainsError(string input)
    {
        // Arrange
        var validator = new SiteFeaturesSearchRequestValidator();
        var request = new SiteFeaturesSearchRequest { Limit = input };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Limit");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Limit must be a number between 1 and 10000.");
    }
}