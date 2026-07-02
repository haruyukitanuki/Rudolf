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

  /// <summary>Sim-known beacon type code → human-readable meaning.</summary>
  public Dictionary<string, string>? Transponders;
}

/// <summary>Panel-lamp vocabulary overrides: typically maps opaque integer panel indices to named keys.</summary>
public class LampVocabulary
{
  /// <summary>Panel-array index (string-keyed) → named lamp key.</summary>
  public Dictionary<string, string>? BveIndexToKey;
}
