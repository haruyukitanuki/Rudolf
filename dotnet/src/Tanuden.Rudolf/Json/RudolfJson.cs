using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Json;

/// <summary>
///   Default <see cref="JsonSerializerOptions" /> for serializing and deserializing RUDOLF documents.
///   Configures camelCase property naming, includes public fields, and registers the discriminated-union
///   converter for <see cref="Tanuden.Rudolf.Input.Command" />. Reuse the singleton via <see cref="Options" />.
/// </summary>
public static class RudolfJson
{
  /// <summary>Singleton <see cref="JsonSerializerOptions" /> configured for RUDOLF documents.</summary>
  public static JsonSerializerOptions Options { get; } = Build();

  private static JsonSerializerOptions Build()
  {
    var options = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
      IncludeFields = true,
      DefaultIgnoreCondition = JsonIgnoreCondition.Never,
      WriteIndented = false,
      // Emit non-ASCII (Japanese station/route/vehicle names) as literal UTF-8 rather than \uXXXX
      // escapes. RUDOLF is an IPC/MMF/WebSocket wire consumed by JSON.parse / JsonSerializer.Deserialize,
      // never injected raw into HTML, so the relaxed encoder's only caveat (HTML/script embedding) does
      // not apply. Set before first use, while the options are still mutable.
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    // Last-resort net: coerce any non-finite double (NaN/±Infinity) to a valid JSON number so one bad
    // value can never abort a whole document. Finite values are unaffected. See SanitizingDoubleConverter.
    options.Converters.Add(new SanitizingDoubleConverter());

    return options;
  }
}
