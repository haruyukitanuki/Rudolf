using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tanuden.Rudolf.Input;

namespace Tanuden.Rudolf.Json;

/// <summary>
///   Discriminated-union converter for <see cref="Command" />. Reads/writes a <c>kind</c> string
///   discriminator and dispatches to the appropriate <c>SetXxxCommand</c> subclass.
/// </summary>
public class CommandJsonConverter : JsonConverter<Command>
{
  /// <inheritdoc />
  public override Command Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    using var doc = JsonDocument.ParseValue(ref reader);
    var root = doc.RootElement;
    if (!root.TryGetProperty("kind", out var kindElem) || kindElem.ValueKind != JsonValueKind.String)
      throw new JsonException("InputCommand.command missing 'kind' string discriminator");

    var kind = kindElem.GetString();
    var raw = root.GetRawText();

    return kind switch
    {
      "SetNotch" => Deserialize<SetNotchCommand>(raw, options),
      "SetPowerNotch" => Deserialize<SetPowerNotchCommand>(raw, options),
      "SetBrakeNotch" => Deserialize<SetBrakeNotchCommand>(raw, options),
      "SetBrakeSAP" => Deserialize<SetBrakeSAPCommand>(raw, options),
      "SetReverser" => Deserialize<SetReverserCommand>(raw, options),
      "SetButton" => Deserialize<SetButtonCommand>(raw, options),
      "SetWiper" => Deserialize<SetWiperCommand>(raw, options),
      "SetAtoNotch" => Deserialize<SetAtoNotchCommand>(raw, options),
      "SetDeadman" => Deserialize<SetDeadmanCommand>(raw, options),
      _ => throw new JsonException($"Unknown command kind: {kind}")
    };
  }

  /// <inheritdoc />
  public override void Write(Utf8JsonWriter writer, Command value, JsonSerializerOptions options)
  {
    switch (value)
    {
      case SetNotchCommand c:
        writer.WriteStartObject();
        writer.WriteString("kind", "SetNotch");
        writer.WriteNumber("value", c.Value);
        writer.WriteEndObject();
        break;
      case SetPowerNotchCommand c:
        writer.WriteStartObject();
        writer.WriteString("kind", "SetPowerNotch");
        writer.WriteNumber("value", c.Value);
        writer.WriteEndObject();
        break;
      case SetBrakeNotchCommand c:
        writer.WriteStartObject();
        writer.WriteString("kind", "SetBrakeNotch");
        writer.WriteNumber("value", c.Value);
        writer.WriteEndObject();
        break;
      case SetBrakeSAPCommand c:
        writer.WriteStartObject();
        writer.WriteString("kind", "SetBrakeSAP");
        writer.WriteNumber("kPa", c.KPa);
        writer.WriteEndObject();
        break;
      case SetReverserCommand c:
        writer.WriteStartObject();
        writer.WriteString("kind", "SetReverser");
        writer.WriteNumber("value", (int)c.Value);
        writer.WriteEndObject();
        break;
      case SetButtonCommand c:
        writer.WriteStartObject();
        writer.WriteString("kind", "SetButton");
        writer.WritePropertyName("action");
        JsonSerializer.Serialize(writer, c.Action, options);
        writer.WriteBoolean("state", c.State);
        writer.WriteEndObject();
        break;
      case SetWiperCommand c:
        writer.WriteStartObject();
        writer.WriteString("kind", "SetWiper");
        writer.WritePropertyName("state");
        JsonSerializer.Serialize(writer, c.State, options);
        writer.WriteEndObject();
        break;
      case SetAtoNotchCommand c:
        writer.WriteStartObject();
        writer.WriteString("kind", "SetAtoNotch");
        writer.WriteNumber("value", c.Value);
        writer.WriteEndObject();
        break;
      case SetDeadmanCommand c:
        writer.WriteStartObject();
        writer.WriteString("kind", "SetDeadman");
        writer.WritePropertyName("channel");
        JsonSerializer.Serialize(writer, c.Method, options);
        writer.WriteBoolean("holding", c.Holding);
        writer.WriteEndObject();
        break;
      default:
        throw new JsonException($"Unknown command type: {value.GetType().FullName}");
    }
  }

  private static T Deserialize<T>(string raw, JsonSerializerOptions options) where T : Command
  {
    var result = JsonSerializer.Deserialize<T>(raw, options);
    return result ?? throw new JsonException($"Failed to deserialize {typeof(T).Name}");
  }
}
