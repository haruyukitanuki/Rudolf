using System.Collections.Generic;

namespace Tanuden.Rudolf.Profile;

/// <summary>
///   Vocabulary overrides for this sim+vehicle. Lets a consumer translate sim-specific
///   indices (panel-lamp arrays, custom signal aspects, beacon codes) into
///   human-meaningful labels without hard-coding per-vehicle knowledge in the UI.
/// </summary>
public class Vocabularies
{
  /// <summary>Panel-lamp index → name overrides; null when no overrides apply.</summary>
  public Dictionary<string, string> Lamps = new Dictionary<string, string>
  {
    { "0", "doorClose" },
    { "1", "atsReady" },
    { "2", "atsBrakeApply" },
    { "3", "atsOpen" },
    { "4", "regenerative" },
    { "5", "ebTimer" },
    { "6", "emergencyBrake" },
    { "7", "overload" },
    { "8", "ato" },
    { "9", "snowBrake" },
    { "10", "wheelSlip" },
  };

  /// <summary>Per-route overrides for the default 0-7 signal-phase vocabulary.</summary>
  public Dictionary<string, string>? SignalPhase = new Dictionary<string, string>
  {
    { "0", ":" },
    { "1", "R" },
    { "2", "YY" },
    { "3", "Y" },
    { "4", "YG" },
    { "5", "YGF" },
    { "6", "G" },
    { "7", "GG" },
  };

  /// <summary>
  ///   Per-route signal-phase-speed vocabulary. Maps Rudolf phase-index-as-string
  ///   ("1".."7" and any "8+" override) to a km/h cap. Value convention:
  ///   <c>n &gt;= 0</c> = km/h cap; <c>-1</c> = unlimited (no inherent cap, line speed);
  ///   <c>null</c> = unknown. Consumers fall back to the spec's default-speed table for keys
  ///   absent from this map. Populated by both adapters at profile-emit time.
  /// </summary>
  public Dictionary<string, double?>? SignalPhaseSpeed = new Dictionary<string, double?>
  {
    { "0", -1 },
    { "1", 0 },
    { "2", 25 },
    { "3", 45 },
    { "4", 65 },
    { "5", 90 },
    { "6", -1 },
    { "7", -1 },
  };

  /// <summary>Sim-known beacon type code → human-readable meaning.</summary>
  public Dictionary<string, string>? Transponders;
}
