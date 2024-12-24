namespace Jacobi.AdventureBuilder.GameContracts;

internal static class Key
{
    public const char Separator = ';';

    public static string[] Split(string key)
        => key.Split(Separator);

    public static string Join(params object[] parts)
        => String.Join(Separator, parts);
}
