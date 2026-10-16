using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Tickets.Tests.Support;

/// <summary>
/// Issues JWTs for the tests, signed with a key that only the tests know.
/// TicketsApiFactory configures the API to trust that key, whatever `dotnet user-jwts` wrote on the machine.
/// </summary>
public static class TestTokens
{
    public const string Issuer = "tp-net-tests";
    public const string Audience = "tickets-api";

    public static readonly SymmetricSecurityKey SigningKey =
        new(Encoding.UTF8.GetBytes("tp-net-test-signing-key-at-least-32-bytes-long"));

    public static string For(string name, params string[] roles) => Create(name, roles, SigningKey);

    /// <summary>A token signed with another key: the API must refuse it.</summary>
    public static string Forged(string name) =>
        Create(name, [], new SymmetricSecurityKey(Encoding.UTF8.GetBytes("a-key-the-api-has-never-seen-before-32-bytes")));

    private static string Create(string name, string[] roles, SecurityKey key) =>
        new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Expires = DateTime.UtcNow.AddMinutes(30),
            Subject = new ClaimsIdentity([new Claim("sub", name), new Claim("unique_name", name), .. roles.Select(role => new Claim("role", role))]),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256),
        });
}
