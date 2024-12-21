using System.Diagnostics.CodeAnalysis;
using LanguageExt;

internal static class OptionExtensions
{
    public static bool TryGetValue<T>(this Option<T> option, [NotNullWhen(true)] out T? value)
    {
        var arr = option.ToArray();
        if (arr.Length == 1)
        {
            value = arr[0]!;
            return true;
        }

        value = default;
        return false;
    }
}
