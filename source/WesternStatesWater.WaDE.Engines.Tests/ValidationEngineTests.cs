using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WesternStatesWater.WaDE.Engines.Contracts;

namespace WesternStatesWater.WaDE.Engines.Tests;

[TestClass]
public class ValidationEngineTests
{
    private readonly IValidationEngine _validationEngine = new ValidationEngine();

    // Go ahead and remove me when a real test is added.
    [TestMethod]
    public void SmokeTest()
    {
        _validationEngine.Should().NotBeNull();
    }
}