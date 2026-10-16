using System.Reflection;

namespace Katas.Tests.Exercise07;

public class MyTestsCheck
{
    [Fact]
    public void MyTests_ContainsAtLeastThreeTests()
    {
        var tests = typeof(MyTests)
            .GetMethods()
            .Count(method => method.GetCustomAttribute<FactAttribute>(inherit: true) is not null);

        Assert.True(tests >= 3, $"MyTests contains {tests} test(s); write at least 3.");
    }
}
