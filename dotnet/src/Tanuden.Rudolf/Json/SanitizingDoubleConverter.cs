using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Json;

/// <summary>
///   Last-resort safety net that keeps a non-finite <see cref="double" /> (<c>NaN</c> or ±Infinity) from
///   aborting serialization of an entire RUDOLF document. <see cref="System.Text.Json" /> throws on
///   non-finite numbers by default, so a single such value — e.g. an unlimited BVE speed limit read as
///   +Infinity — would otherwise crash the producer's write path and stall the wire. Finite values pass
///   through byte-for-byte identical; non-finite values degrade to <c>0</c> (valid JSON that round-trips
///   into a non-nullable <see cref="double" />). Semantic special cases (such as "no speed limit") should
///   be resolved at the source; this converter only guarantees the stream never dies.
/// </summary>
public sealed class SanitizingDoubleConverter : JsonConverter<double>
{
  /// <inheritdoc />
  public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return reader.GetDouble();
  }

  /// <inheritdoc />
  public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
  {
    var safe = double.IsNaN(value) || double.IsInfinity(value) ? 0.0 : value;
    writer.WriteNumberValue(safe);
  }
}
