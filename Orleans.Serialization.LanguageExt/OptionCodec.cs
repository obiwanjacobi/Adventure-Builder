using System.Buffers;
using LanguageExt;
using Orleans.Serialization.Buffers;
using Orleans.Serialization.Codecs;
using Orleans.Serialization.Serializers;
using Orleans.Serialization.WireProtocol;

namespace Orleans.Serializers.LanguageExt;

//
// NOT IN USE
//

//[RegisterSerializer]
public sealed class OptionCodec<T> : IFieldCodec<Option<T>>
{
    private readonly IFieldCodec<T> _valueCodec;

    public OptionCodec(IFieldCodecProvider codecProvider)
    {
        _valueCodec = codecProvider.GetCodec<T>();
    }

    public void WriteField<TBufferWriter>(ref Writer<TBufferWriter> writer, uint fieldIdDelta, Type expectedType, Option<T> value) where TBufferWriter : IBufferWriter<byte>
    {
        writer.WriteFieldHeader(fieldIdDelta, expectedType, typeof(Option<T>), WireType.TagDelimited);
        if (value.IsSome)
        {
            writer.WriteByte(1);
            _valueCodec.WriteField(ref writer, 0, typeof(T), value);
        }
        else
        {
            writer.WriteByte(0);
        }
        writer.WriteEndObject();
    }

    public Option<T> ReadValue<TInput>(ref Reader<TInput> reader, Field field)
    {
        reader.ReadFieldHeader();
        var hasValue = reader.ReadByte();
        if (hasValue == 1)
        {
            var value = _valueCodec.ReadValue(ref reader, field);
            return Option<T>.Some(value);
        }
        return Option<T>.None;
    }
}
