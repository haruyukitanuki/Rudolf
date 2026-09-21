using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Where a bogie sits under its car, in left-to-right display order. Non-bogie fixed-axle groups use the same values (they draw identically).</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BogiePosition
{
  /// <summary>Leftmost bogie under the car as displayed.</summary>
  Left,

  /// <summary>Intermediate bogie (3+ bogie cars, e.g. Bo-Bo-Bo).</summary>
  Middle,

  /// <summary>Rightmost bogie under the car as displayed.</summary>
  Right,

  /// <summary>Bogie shared with the adjacent car (Jacobs bogie, articulated/AGT/LRV).</summary>
  Jacobs
}
