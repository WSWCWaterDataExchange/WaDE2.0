using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

namespace WesternStatesWater.WaDE.Managers.Tests.Validators.V2;

[TestClass]
public class OverlayFeaturesSearchRequestValidatorTests
{
    [TestMethod]
    public void Validator_IsValid_ReturnsTrue()
    {
        // Arrange
        var validator = new OverlayFeaturesItemRequestValidator();
        var request = new OverlayFeaturesItemRequest { Bbox = "1,2,3,4" };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [DataTestMethod]
    [DataRow(null, true)]
    [DataRow("POLYGON ((30 10, 40 40, 20 40, 10 20, 30 10))", true)]
    [DataRow("POLYGON((30 10,40 40,20 40,10 20,30 10))", true)]
    [DataRow("polygon ((30 10, 40 40, 20 40, 10 20, 30 10))", true)]
    [DataRow("multiPOLYGON     (((30 20, 45 40, 10 40, 30 20)), ((15 5, 40 10, 10 20, 5 10, 15 5)))     ", true)]
    [DataRow("MULTIPOLYGON (((30 20, 45 40, 10 40, 30 20)), ((15 5, 40 10, 10 20, 5 10, 15 5)))", true)]
    [DataRow("", false)]
    [DataRow("POINT (30 10)", false)]
    [DataRow("MULTIPOLYGON (((30 20, 45 40, 10 40, 30 20)), ((15 5, 40 10, 10 20, 5 10)))", false)] // Missing closing coordinates
    public void Validator_ShouldReturnExpectedValidity_ForGivenWktStrings(string wktString, bool expected)
    {
        // Arrange
        var validator = new OverlayFeaturesAreaRequestValidator();
        var request = new OverlayFeaturesAreaRequest { Coords = wktString };
        
        // Act
        var result = validator.Validate(request);
        
        // Assert
        result.IsValid.Should().Be(expected);
    }

    [DataTestMethod]
    [DataRow("", "'Coords' must not be empty.")]
    [DataRow("POINT (1 2)", "'Coords' must be a valid POLYGON or MULTIPOLYGON string.")]
    [DataRow("POLYGON ((1 1, 2 2)), ", "Invalid 'Coords'. Ensure the string is a valid POLYGON or MULTIPOLYGON WKT format, e.g., POLYGON((x y,x1 y1,x2 y2,...,xn yn x y)).")]
    public void Validator_CoordsIsFalse_ReturnsMessage(string wktString, string errorMessage)
    {
        // Arrange
        var validator = new OverlayFeaturesAreaRequestValidator();
        var request = new OverlayFeaturesAreaRequest { Coords = wktString };
        
        // Act
        var result = validator.Validate(request);
        
        // Assert
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == errorMessage);
    }
}