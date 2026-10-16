namespace Tickets.Web.Security;

/// <summary>
/// Provided. Demo accounts, so that the lab needs no identity provider.
/// In a real application, the login goes through OpenID Connect to the company's identity provider
/// (Entra ID, Keycloak…) and no password ever reaches our code.
/// </summary>
public static class DemoUsers
{
    public sealed record DemoUser(string Name, string[] Roles);

    private static readonly Dictionary<string, DemoUser> Users = new(StringComparer.OrdinalIgnoreCase)
    {
        ["alice"] = new("alice", []),
        ["admin"] = new("admin", ["admin"]),
    };

    /// <summary>Every demo account has the password "demo".</summary>
    public static DemoUser? Find(string userName, string password) =>
        password == "demo" && Users.TryGetValue(userName, out var user) ? user : null;
}
