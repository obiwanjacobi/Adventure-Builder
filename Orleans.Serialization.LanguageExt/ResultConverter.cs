using System.Diagnostics;
using LanguageExt.Common;

namespace Orleans.Serialization.LanguageExt;

[RegisterConverter]
public sealed class ResultConverter<T> : IConverter<Result<T>, ResultSurrogate<T>>
{
    public Result<T> ConvertFromSurrogate(in ResultSurrogate<T> surrogate)
    {
        if (surrogate.error is not null)
            return new Result<T>(surrogate.error);
        if (surrogate.instance is not null)
            return new Result<T>(surrogate.instance);

        Debug.Fail("Serialization ResultSurrogate<T> did not have a value set.");
        return new Result<T>();
    }

    public ResultSurrogate<T> ConvertToSurrogate(in Result<T> value)
    {
        return value.Match(
            Succ: instance => new ResultSurrogate<T>(instance),
            Fail: error => new ResultSurrogate<T>(error)
        );
    }
}

[GenerateSerializer, Immutable]
public struct ResultSurrogate<T>
{
    public ResultSurrogate(T instance)
    { this.instance = instance; }
    public ResultSurrogate(Exception error)
    { this.error = error; }

    [Id(0)]
    public Exception? error { get; }
    [Id(1)]
    public T? instance { get; }
}
