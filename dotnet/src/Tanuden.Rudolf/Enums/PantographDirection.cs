using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>
///   Which end of the car a pantograph leans toward (HMI-relative, matching <see cref="Direction" />);
///   <c>Both</c> = pantographs/lean toward both ends.
/// </summary>
[JsonConverter(typeof(Tanuden.Rudolf.Json.StringOnlyEnumConverter))]
public enum PantographDirection
{
  /// <summary>Leans toward the left on HMI.</summary>
  Left,

  /// <summary>Leans toward the right on HMI.</summary>
  Right,

  /// <summary>Leans toward both ends.</summary>
  Both
}
