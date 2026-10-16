using System.Globalization;
using Humanizer;

namespace Katas.Exercise02;

/// <summary>
/// Exercise 02: use a NuGet package (Humanizer) through Central Package Management.
/// </summary>
public static class TextFormatter
{
    // Humanizer ships English resources only in Humanizer.Core; pinning the culture keeps
    // the output identical whatever the machine's regional settings are.
    private static readonly CultureInfo English = CultureInfo.GetCultureInfo("en-US");

    /// <summary>Formats a duration for humans, e.g. 125 minutes → "2 hours, 5 minutes".</summary>
    public static string DescribeDuration(TimeSpan duration) =>
        duration.Humanize(precision: 2, culture: English);

    /// <summary>Turns an identifier into a sentence, e.g. "OrderLineCount" → "Order line count".</summary>
    public static string ToSentence(string identifier) =>
        identifier.Humanize(LetterCasing.Sentence);
}
