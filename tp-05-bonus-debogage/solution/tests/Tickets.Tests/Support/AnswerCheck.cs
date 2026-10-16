using System.Security.Cryptography;
using System.Text;

namespace Tickets.Tests.Support;

/// <summary>
/// The expected answers are stored as hashes, so that reading the tests does not give them away.
/// </summary>
public static class AnswerCheck
{
    public static string Hash(string answer) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(answer.Trim().ToLowerInvariant())))[..16];
}
