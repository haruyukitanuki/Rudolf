using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>
///   Which end of the car a pantograph leans toward (TIMS-relative, matching <see cref="Direction" />);
///   <c>Both</c> = pantographs/lean toward both ends.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PantographDirection
{
  /// <summary>Leans toward the left (TIMS-relative).</summary>
  Left,

  /// <summary>Leans toward the right (TIMS-relative).</summary>
  Right,

  /// <summary>Leans toward both ends.</summary>
  Both
}
