namespace Tanuden.Rudolf.Enums;

/// <summary>
///   Which side(s) of a car are currently open. Serialized as its underlying int.
///   <c>-1 = Left, 0 = Closed, 1 = Right, 2 = Both, 3 = Open (side unknown)</c>.
///   <c>null</c> means no per-car door value is available (spec §3.1), not "open, side unknown".
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
  Both = 2,

  /// <summary>Doors open but which side can't be distinguished.</summary>
  OpenSideUnknown = 3
}
