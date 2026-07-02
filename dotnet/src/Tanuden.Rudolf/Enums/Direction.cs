using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Direction relative on TIMS screen</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Direction
{
  /// <summary>Facing left on TIMS</summary>
  Left,

  /// <summary>Facing right on TIMS</summary>
  Right
}
