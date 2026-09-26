using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Where a bogie sits under its car, in left-to-right display order. Non-bogie fixed-axle groups use the same values (they draw identically).</summary>
[JsonConverter(typeof(Tanuden.Rudolf.Json.StringOnlyEnumConverter))]
public enum BogiePosition
{
  /// <summary>Leftmost bogie under the car as displayed.</summary>
  Left,

  /// <summary>Intermediate bogie (only for cars with 3 or more bogies, e.g. Bo-Bo-Bo).</summary>
  Middle,

  /// <summary>Rightmost bogie under the car as displayed.</summary>
  Right,

  /// <summary>Bogie shared with the next car (Jacobs/articulated bogie).</summary>
  Jacobs
}
