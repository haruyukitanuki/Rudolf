using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>
///   Origin of a speed-limit value.
/// </summary>
[JsonConverter(typeof(Tanuden.Rudolf.Json.StringOnlyEnumConverter))]
public enum SpeedLimitType
{
  /// <summary>Imposed by an upcoming signal aspect.</summary>
  Signal,

  /// <summary>Posted safe limit for stretch of track.</summary>
  SpeedLimit,

  /// <summary>Temporary speed restricted section.</summary>
  Restriction
}
