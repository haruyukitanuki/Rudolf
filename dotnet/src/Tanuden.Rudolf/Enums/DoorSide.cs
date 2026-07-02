namespace Tanuden.Rudolf.Enums;

/// <summary>
///   Which side the doors are expected to open at a station. Serialized as its underlying int.
///   <c>-1 = Left, 0 = None/Unknown, 1 = Right, 2 = Both</c>
/// </summary>
public enum DoorSide
{
  /// <summary>Doors open on the left side.</summary>
  Left = -1,

  /// <summary>No door opening expected, or the side is unknown.</summary>
  None = 0,

  /// <summary>Doors open on the right side.</summary>
  Right = 1,

  /// <summary>Doors open on both sides.</summary>
  Both = 2
}
