namespace Tanuden.Rudolf.Enums;

/// <summary>
///   Which side(s) of a car are currently open. Serialized as its underlying int.
///   <c>-1 = Left, 0 = Closed, 1 = Right, 2 = Both</c>.
///   If null is used, it indicates opened but which side is unknown.
/// </summary>
public enum SideOpened
{
  /// <summary>Left doors opened.</summary>
  Left = -1,

  /// <summary>Closed.</summary>
  Closed = 0,

  /// <summary>Right doors opened.</summary>
  Right = 1,

  /// <summary>Doors on both sides opened.</summary>
  Both = 2
}
