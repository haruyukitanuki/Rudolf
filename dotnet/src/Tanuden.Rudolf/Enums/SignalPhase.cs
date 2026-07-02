namespace Tanuden.Rudolf.Enums;

/// <summary>
///   Signal aspect index. Serialized as its underlying int.
///   Proceed-ordered (more permissive = higher number); <c>0</c> reserved for non-functional signals.
///   <list type="bullet">
///     <item><c>0</c> = non-functional or disabled</item>
///     <item><c>1</c> = R (停止)</item>
///     <item><c>2</c> = YY (警戒)</item>
///     <item><c>3</c> = Y (注意)</item>
///     <item><c>4</c> = YG (減速)</item>
///     <item><c>5</c> = YGF (抑速)</item>
///     <item><c>6</c> = G (進行)</item>
///     <item><c>7</c> = GG (高速進行)</item>
///     <item><c>8+</c> = sim/vehicle-specific (see <c>SimulatorProfile.Vocabularies.SignalPhase</c>)</item>
///   </list>
/// </summary>
public enum SignalPhase
{
  /// <summary>Signal is non-functional or disabled</summary>
  Disabled = 0,

  /// <summary>Stop (停止)</summary>
  R = 1,

  /// <summary>Restricted speed (警戒)</summary>
  YY = 2,

  /// <summary>Caution (注意)</summary>
  Y = 3,

  /// <summary>Reduced speed (減速)</summary>
  YG = 4,

  /// <summary>Limited speed (抑速)</summary>
  YGF = 5,

  /// <summary>Proceed (進行)</summary>
  G = 6,

  /// <summary>High-speed Proceed (高速進行)</summary>
  GG = 7
}
