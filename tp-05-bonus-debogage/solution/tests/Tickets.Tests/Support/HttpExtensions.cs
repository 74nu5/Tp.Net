using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Tickets.Tests.Support;

/// <summary>Helpers that read responses as JSON nodes, so the tests do not depend on the C# contract types.</summary>
public static class HttpExtensions
{
    public static async Task<JsonNode> ReadJsonAsync(this HttpResponseMessage response) =>
        JsonNode.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken))!;

    public static async Task<JsonNode> CreateTicketAsync(this HttpClient client, string title, string priority = "Normal")
    {
        var response = await client.PostAsJsonAsync("/tickets", new { title, description = "Created by a test", priority }, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.ReadJsonAsync();
    }

    /// <summary>A title that no other test uses, to find our own tickets in a shared database.</summary>
    public static string UniqueTitle(string prefix) => $"{prefix} {Guid.NewGuid():N}"[..Math.Min(prefix.Length + 9, 200)];
}
