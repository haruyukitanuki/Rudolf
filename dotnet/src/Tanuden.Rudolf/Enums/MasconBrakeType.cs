using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>
///   How the brake handle behaves on the mascon.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MasconBrakeType
{
  /// <summary>Discrete, self-lapping stepped brake notches.</summary>
  Notched,

  /// <summary>Continuous (non-notched) brake handle with no lap position.</summary>
  Continuous,

  /// <summary>Continuous with a lap position for holding pressure.</summary>
  LapCapable
}
