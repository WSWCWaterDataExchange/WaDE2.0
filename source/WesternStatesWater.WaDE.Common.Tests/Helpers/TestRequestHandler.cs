using WesternStatesWater.WaDE.Common.Contracts;

namespace WesternStatesWater.WaDE.Common.Tests.Helpers;

public class TestRequestHandler : IRequestHandler<TestRequest, TestResponse>
{
    public Task<TestResponse> Handle(TestRequest request) => Task.FromResult(new TestResponse());
}