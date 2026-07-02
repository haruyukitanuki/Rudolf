namespace Tanuden.Rudolf.Enums;

/// <summary>
///   Reverser position.
///   Adapters MUST clamp out-of-range sim values into this set. All other values are not acceptable.
/// </summary>
public enum Reverser
{
  /// <summary>Reverse (後進)</summary>
  Reverse = -1,

  /// <summary>Neutral (中立)</summary>
  Neutral = 0,

  /// <summary>Forward (前進)</summary>
  Forward = 1
}
