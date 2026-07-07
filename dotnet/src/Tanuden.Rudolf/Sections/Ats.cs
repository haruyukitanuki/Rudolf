using System;
using Tanuden.Rudolf.Enums;

namespace Tanuden.Rudolf.Sections;

/// <summary>
///   ATS/ATC state.
///   Covers the speed cap currently asserted by ATS plus an optional rich-state object
///   for richer per-family info (P established, EB engaged, etc.).
/// </summary>
public class Ats
{
  /// <summary>Free-form class identifier when the sim natively exposes one; null otherwise.</summary>
  public string? Class;

  /// <summary>
  ///   Current ATS speed limit in km/h. <c>-1</c> = free, <c>null</c> = blank display,
  ///   any other number = the asserted cap.
  /// </summary>
  public double? Speed;

  /// <summary>Free-form rich-state string (e.g. <c>"P接近"</c>, <c>"B動作"</c>, <c>"EB"</c>); null when not asserted.</summary>
  public string? State;

  /// <summary>Structured rich-state alternative to <see cref="State" />; null when the family profile doesn't fill it.</summary>
  public AtsRichState? RichState;
}

/// <summary>
///   Machine-readable ATS events (alternative to the free-form <see cref="Ats.State" /> string).
///   Parallel arrays: index N across all fields describes the Nth active state.
/// </summary>
public class AtsRichState
{
  /// <summary>Stable enum-like codes (e.g. <c>"P_APPROACH"</c>, <c>"EB"</c>).</summary>
  public string[] Code = Array.Empty<string>();

  /// <summary>Human-readable display labels (e.g. <c>"P接近"</c>).</summary>
  public string[] Name = Array.Empty<string>();

  /// <summary>0 = info, 1 = warning, 2 = critical; values above 2 are sim/vehicle-specific custom severities.</summary>
  public int[] Severity = Array.Empty<int>();

  /// <summary>Machine-readable event category for each state (parallel with <see cref="Code" />).</summary>
  public AtsRichStateType[] Type = Array.Empty<AtsRichStateType>();
}
