using System.Diagnostics;

namespace Jacobi.AdventureBuilder.ServiceDefaults;

/// <summary>
/// Name=Value\n
/// Does NOT support parsing quotes in the value (will be part of value).
/// Does support comments, '#' at start of line.
/// </summary>
public static class DotEnv
{
    /// <summary>
    /// Loads '.env' from the current directory.
    /// </summary>
    public static void Load()
        => Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Could not find Environment Variable file '{filePath}'.");
            return;
        }

        Console.WriteLine($"Reading Environment Variables from '{filePath}'.");
        foreach (var line in File.ReadAllLines(filePath))
        {
            if (line.StartsWith('#')) continue;

            var parts = line.Split('=', StringSplitOptions.TrimEntries);

            if (parts.Length != 2)
            {
                Console.WriteLine($"Can't parse Environment Variable line '{line}', skipping.");
                continue;
            }

            var name = parts[0];
            var value = parts[1];
            Debug.WriteLine($"Setting Environment Variable {name}={value}.");
            Environment.SetEnvironmentVariable(name, value);
        }
    }
}