using System.Reflection;

namespace Tickets.Web.Tests.Exercise07;

public class MyComponentTestsCheck
{
    [Fact]
    public void MyComponentTests_ContainsAtLeastThreeTests()
    {
        var tests = typeof(MyComponentTests)
            .GetMethods()
            .Count(method => method.GetCustomAttribute<FactAttribute>(inherit: true) is not null);

        Assert.True(tests >= 3, $"MyComponentTests contains {tests} test(s); write at least 3.");
    }
}
