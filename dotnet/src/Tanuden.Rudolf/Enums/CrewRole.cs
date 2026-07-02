using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Role the player is currently performing.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CrewRole
{
  /// <summary>Driver</summary>
  Driver,

  /// <summary>Conductor</summary>
  Conductor,

  /// <summary>Both Driver and Conductor. This usually indicates one-man operations.</summary>
  Both,

  /// <summary>Others</summary>
  Others
}
