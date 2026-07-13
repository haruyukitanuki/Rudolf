using System.Collections.Generic;

namespace Tanuden.Rudolf.Profile;

/// <summary>
///   Vocabulary overrides for this sim+vehicle. Lets a consumer translate sim-specific
///   indices (panel-lamp arrays, custom signal aspects, beacon codes) into
///   human-meaningful labels without hard-coding per-vehicle knowledge in the UI.
/// </summary>
public class Vocabularies
{
  /// <summary>Panel-lamp vocabulary overrides; null when no overrides apply.</summary>
  public LampVocabulary? Lamps;

  /// <summary>Per-route overrides for the default 0-7 signal-phase vocabulary.</summary>
  public Dictionary<string, string>? SignalPhase;

  /// <summary>
  ///   Per-route signal-phase-speed vocabulary. Maps Rudolf phase-index-as-string
  ///   ("1".."7" and any "8+" override) to a km/h cap. Value convention:
  ///   <c>n &gt;= 0</c> = km/h cap; <c>-1</c> = unlimited (no inherent cap, line speed);
  ///   <c>null</c> = unknown. Consumers fall back to the spec's default-speed table for keys
  ///   absent from this map. Populated by both adapters at profile-emit time.
  /// </summary>
  public Dictionary<string, double?>? SignalPhaseSpeed;

  /// <summary>Sim-known beacon type code → human-readable meaning.</summary>
  public Dictionary<string, string>? Transponders;
}

/// <summary>Panel-lamp vocabulary overrides: typically maps opaque integer panel indices to named keys.</summary>
public class LampVocabulary
{
  /// <summary>Panel-array index (string-keyed) → named lamp key.</summary>
  public Dictionary<string, string>? BveIndexToKey;
}
