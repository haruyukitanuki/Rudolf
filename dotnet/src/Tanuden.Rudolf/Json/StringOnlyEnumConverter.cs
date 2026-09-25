using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Json;

/// <summary>
///   <see cref="JsonStringEnumConverter" /> restricted to the wire form the spec defines: name strings.
///   Numeric aliases (e.g. <c>1</c> for <c>Low</c>) throw <see cref="System.Text.Json.JsonException" />
///   when reading. Writing is unchanged and always emits name strings. Enums that serialize as int on
///   the wire (<c>Reverser</c>, <c>SideOpened</c>) intentionally do NOT use this converter.
/// </summary>
public class StringOnlyEnumConverter : JsonStringEnumConverter
{
  /// <summary>Configures the converter to reject integer wire values.</summary>
  public StringOnlyEnumConverter()
    : base(namingPolicy: null, allowIntegerValues: false)
  {
  }
}
