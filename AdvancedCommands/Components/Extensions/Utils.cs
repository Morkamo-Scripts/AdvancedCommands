using System;
using System.IO;

namespace AdvancedCommands.Components.Extensions;

public static class Utils
{
    public static string GetRoleFromRemoteAdminConfig(string configPath, string userId)
    {
        var lines = File.ReadAllLines(configPath);

        bool inMembers = false;

        foreach (var raw in lines)
        {
            var line = raw.Trim();

            if (line == "Members:")
            {
                inMembers = true;
                continue;
            }

            if (inMembers)
            {
                if (!line.StartsWith("-"))
                    break;

                var entry = line.Substring(1).Trim();

                var parts = entry.Split(':', 2);
                if (parts.Length != 2)
                    continue;

                var id = parts[0].Trim();
                var role = parts[1].Trim();

                if (string.Equals(id, userId, StringComparison.OrdinalIgnoreCase))
                    return role;
            }
        }

        return null;
    }
}