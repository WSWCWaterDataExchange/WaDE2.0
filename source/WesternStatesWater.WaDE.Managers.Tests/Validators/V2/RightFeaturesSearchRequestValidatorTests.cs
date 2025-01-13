using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Contracts.Api.Requests.V2;

namespace WesternStatesWater.WaDE.Managers.Tests.Validators.V2;

[TestClass]
public class RightFeaturesSearchRequestValidatorTests
{
    [TestMethod]
    public void Validator_IsValid_ReturnsTrue()
    {
        // Arrange
        var validator = new RightFeaturesSearchRequestValidator();
        var request = new RightFeaturesSearchRequest { Bbox = "1,2,3,4" };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}