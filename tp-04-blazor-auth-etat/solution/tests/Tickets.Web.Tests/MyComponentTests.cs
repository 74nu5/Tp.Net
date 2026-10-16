using Tickets.Components;

namespace Tickets.Web.Tests.Exercise07;

/// <summary>
/// TP-04, exercise 07: tests of the library component, written by the apprentice. This is one possible answer.
/// BunitContext gives Render, Find and the fake JavaScript runtime.
/// </summary>
public class MyComponentTests : BunitContext
{
    [Fact]
    public void Badge_RendersItsContent()
    {
        var badge = Render<Badge>(parameters => parameters.AddChildContent("Beta"));

        Assert.Equal("Beta", badge.Find("span").TextContent);
    }

    [Theory]
    [InlineData("warning", "badge-warning")]
    [InlineData("high", "badge-high")]
    public void Badge_TurnsTheVariantIntoACssClass(string variant, string cssClass)
    {
        var badge = Render<Badge>(parameters => parameters.Add(b => b.Variant, variant).AddChildContent("x"));

        Assert.Contains(cssClass, badge.Find("span").ClassList);
    }

    [Fact]
    public void Badge_IsNeutral_ByDefault()
    {
        var badge = Render<Badge>(parameters => parameters.AddChildContent("x"));

        Assert.Contains("badge-neutral", badge.Find("span").ClassList);
    }

    [Fact]
    public void Badge_RendersMarkupContent()
    {
        var badge = Render<Badge>(parameters => parameters.AddChildContent("<strong>New</strong>"));

        Assert.Equal("New", badge.Find("span > strong").TextContent);
    }
}
