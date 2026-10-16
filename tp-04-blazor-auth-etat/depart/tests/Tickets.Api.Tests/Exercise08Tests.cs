using System.Reflection;

namespace Tickets.Api.Tests.Exercise08;

public class MyApiTestsCheck
{
    [Fact]
    public void MyApiTests_ContainsAtLeastTwoTests()
    {
        var tests = typeof(MyApiTests)
            .GetMethods()
            .Count(method => method.GetCustomAttribute<FactAttribute>(inherit: true) is not null);

        Assert.True(tests >= 2, $"MyApiTests contains {tests} test(s); write at least 2.");
    }
}
