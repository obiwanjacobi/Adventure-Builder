using LanguageExt;

namespace Orleans.Serialization.LanguageExt;

[RegisterConverter]
public sealed class OptionConverter<T> : IConverter<Option<T>, OptionSurrogate<T>>
{
    public Option<T> ConvertFromSurrogate(in OptionSurrogate<T> surrogate)
    {
        if (surrogate.some == 1)
            return Option<T>.Some(surrogate.instance!);
        return Option<T>.None;
    }

    public OptionSurrogate<T> ConvertToSurrogate(in Option<T> value)
    {
        return value.Match(
            Some: value => new OptionSurrogate<T>(value),
            None: () => new OptionSurrogate<T>()
        );
    }
}

[GenerateSerializer, Immutable]
public struct OptionSurrogate<T>
{
    public OptionSurrogate(T instance)
    { some = 1; this.instance = instance; }

    [Id(0)]
    public byte some { get; }
    [Id(1)]
    public T? instance { get; }
}
