using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Direction relative on HMI screen</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Direction
{
  /// <summary>Facing left on HMI</summary>
  Left,

  /// <summary>Facing right on HMI</summary>
  Right
}
