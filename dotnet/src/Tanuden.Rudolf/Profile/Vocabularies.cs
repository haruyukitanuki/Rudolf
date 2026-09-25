using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Profile;

/// <summary>
///   Vocabulary overrides for this sim+vehicle. Lets a consumer translate sim-specific
///   indices (panel-lamp arrays, custom signal aspects, beacon codes) into
///   human-meaningful labels without hard-coding per-vehicle knowledge in the UI.
/// </summary>
public class Vocabularies
{
  /// <summary>Panel-lamp name → indexes; null when no overrides apply.</summary>
  public Dictionary<string, int>? Lamps;

  /// <summary>Per-route overrides for the default 0-7 signal-phase vocabulary.</summary>
  public Dictionary<string, string>? SignalPhase;

  /// <summary>
  ///   Per-route signal-phase-speed vocabulary. Maps Rudolf phase-index-as-string
  ///   ("1".."7" and any "8+" override) to a km/h cap. Value convention:
  ///   <c>n &gt;= 0</c> = km/h cap; <c>-1</c> = unlimited (no inherent cap, line speed);
  ///   <c>null</c> = unknown. Consumers fall back to the spec's default-speed table for keys
  ///   absent from this map. Populated by adapters at profile-emit time.
  /// </summary>
  public Dictionary<string, double?>? SignalPhaseSpeed;

  /// <summary>Sim-known beacon type code → human-readable meaning.</summary>
  public Dictionary<string, string>? Transponders;

  /// <summary>Default values for lamps: Panel-lamp name → indexes.</summary>
  [JsonIgnore]
  public static readonly ReadOnlyDictionary<string, int> DefaultLamps = new ReadOnlyDictionary<string, int>
  (
    new Dictionary<string, int>
    {
      { "doorClose", 0 },
      { "atsReady", 1 },
      { "atsBrakeApply", 2 },
      { "atsOpen", 3 },
      { "regenerative", 4 },
      { "ebTimer", 5 },
      { "emergencyBrake", 6 },
      { "overload", 7 },
      { "ato", 8 },
      { "snowBrake", 9 },
      { "wheelSlip", 10 },
    }
  );

  /// <summary>Default values for the 0-7 signal-phase vocabulary.</summary>
  [JsonIgnore]
  public static readonly ReadOnlyDictionary<string, string> DefaultSignalPhase = new ReadOnlyDictionary<string, string>
  (
    new Dictionary<string, string>
    {
      { "0", ":" },
      { "1", "R" },
      { "2", "YY" },
      { "3", "Y" },
      { "4", "YG" },
      { "5", "YGF" },
      { "6", "G" },
      { "7", "GG" },
    }
  );

  /// <summary>Default speed table values for the 0-7 signal phase vocabulary.</summary>
  [JsonIgnore]
  public static readonly ReadOnlyDictionary<string, double?> DefaultSignalPhaseSpeed = new ReadOnlyDictionary<string, double?>
  (
    new Dictionary<string, double?>
    {
      { "0", -1 },
      { "1", 0 },
      { "2", 25 },
      { "3", 45 },
      { "4", 65 },
      { "5", 90 },
      { "6", -1 },
      { "7", -1 },
    }
  );
}
