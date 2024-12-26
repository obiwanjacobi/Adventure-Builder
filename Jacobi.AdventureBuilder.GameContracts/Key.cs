using System.Diagnostics;

namespace Jacobi.AdventureBuilder.GameContracts;

internal static class Key
{
    public const char KeySeparator = ';';
    public const char ValueSeparator = ',';

    /// <summary>
    /// Returns an array of keys and for each key a value array.
    /// </summary>
    public static string[][] Split(string key)
    {
        return key.Split(KeySeparator)
            .Select(key => key.Split(ValueSeparator))
            .ToArray();
    }

    /// <summary>
    /// Join the value parts of a key.
    /// </summary>
    public static string Join(params object[] parts)
        => String.Join(ValueSeparator, parts);

    /// <summary>
    /// Join one key with the parts of another.
    /// </summary>
    public static string Join<T>(T key, params object[] parts) where T : struct
    {
        Debug.Assert(typeof(T).Name.EndsWith("Key"));
        return $"{key}{KeySeparator}{Join(parts)}";
    }

    /// <summary>
    /// Does the key contain the tag followed by a ValueSeparator?
    /// </summary>
    public static bool HasTag(string key, string tag)
        => key.Contains($"{tag}{ValueSeparator}");
}
