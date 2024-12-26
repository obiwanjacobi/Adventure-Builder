namespace Jacobi.AdventureBuilder.GameContracts;

internal static class Key
{
    public const char Separator = ';';

    public static string[] Split(string key)
        => key.Split(Separator);

    public static string Join(params object[] parts)
        => String.Join(Separator, parts);

    public static bool HasTag(string key, string tag)
        => key.Contains($"{Separator}{tag}{Separator}");

    public const char KeySeparator = ';';
    public const char ValueSeparator = ',';

    public static IEnumerable<string[]> Split2(string key)
    {
        var keys = key.Split(KeySeparator);
        return keys.Select(key => key.Split(ValueSeparator));
    }

    public static string Join2(params object[] parts)
        => String.Join(ValueSeparator, parts);

    public static string Join<T>(T key, params object[] parts) where T : struct
        => $"{key}{KeySeparator}{Join2(parts)}";

    public static bool HasTag2(string key, string tag)
        => key.Contains($"{tag}{ValueSeparator}");
}
