using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tickets.Web.Client.Api;

/// <summary>
/// A typed HttpClient: the base address is configured once in Program.cs, and IHttpClientFactory manages the connections.
/// </summary>
public sealed class TicketsClient(HttpClient http) : ITicketsClient
{
    // camelCase properties, and enums as text ("High"), like the API.
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    public async Task<IReadOnlyList<TicketDto>> GetTicketsAsync(CancellationToken cancellationToken = default) =>
        await http.GetFromJsonAsync<List<TicketDto>>("tickets?page=1", Json, cancellationToken) ?? [];

    public async Task<TicketDto> CreateTicketAsync(NewTicketForm ticket, CancellationToken cancellationToken = default)
    {
        var response = await http.PostAsJsonAsync("tickets", ticket, Json, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TicketDto>(Json, cancellationToken))!;
    }
}
