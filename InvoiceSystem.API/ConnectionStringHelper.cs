namespace InvoiceSystem.API;

/// <summary>
/// Converts Render/Heroku-style postgresql:// URIs to Npgsql key=value connection string format.
/// </summary>
internal static class ConnectionStringHelper
{
    public static string ConvertPostgresUriToNpgsqlFormat(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return connectionString;

        var s = connectionString.Trim();
        if (!s.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !s.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return connectionString;
        }

        var uri = new Uri(s);
        var userInfo = uri.UserInfo;
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.AbsolutePath.TrimStart('/').TrimEnd('/');
        if (string.IsNullOrEmpty(database)) database = "postgres";

        string username = "", password = "";
        if (!string.IsNullOrEmpty(userInfo))
        {
            var colonIndex = userInfo.IndexOf(':');
            if (colonIndex >= 0)
            {
                username = Uri.UnescapeDataString(userInfo[..colonIndex]);
                password = Uri.UnescapeDataString(userInfo[(colonIndex + 1)..]);
            }
            else
            {
                username = Uri.UnescapeDataString(userInfo);
            }
        }

        var parts = new List<string>();
        if (!string.IsNullOrEmpty(host)) parts.Add($"Host={host}");
        parts.Add($"Port={port}");
        if (!string.IsNullOrEmpty(database)) parts.Add($"Database={database}");
        if (!string.IsNullOrEmpty(username)) parts.Add($"Username={username}");
        if (!string.IsNullOrEmpty(password)) parts.Add($"Password={password}");
        parts.Add("SSL Mode=Prefer");

        return string.Join(";", parts);
    }
}
