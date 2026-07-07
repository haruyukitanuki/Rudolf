using System.Collections.Generic;
using System.Text.Json;
using Tanuden.Rudolf.Sections;

namespace Tanuden.Rudolf;

/// <summary>
///   Per-frame snapshot of dynamic train and cab state emitted by a simulator (sim → consumer).
///   Sent per-frame at the producer's cadence (~4 Hz typical; producers may emit faster). Pair with a <see cref="SimulatorProfile" /> sharing the same
///   <see cref="ScenarioId" /> for capability flags and vocabulary overrides.
/// </summary>
public class OutputDataFrame
{
  /// <summary>RUDOLF schema version this frame conforms to (see <see cref="Constants.SchemaVersion" />).</summary>
  public string SchemaVersion = Constants.SchemaVersion;

  /// <summary>Document discriminator. Always <c>"OutputDataFrame"</c>.</summary>
  public string Kind = "OutputDataFrame";

  /// <summary>Opaque identifier tying all documents of one play-session together.</summary>
  public string ScenarioId = string.Empty;

  /// <summary>ISO 8601 timestamp at the producer.</summary>
  public string SentAt = string.Empty;

  /// <summary>Sim clock + frame counter.</summary>
  public Time Time = new();

  /// <summary>Train identity (number, bound-for, service class) for this run.</summary>
  public Diagram Diagram = new();

  /// <summary>Ordered station list for the diagram plus current/next indices.</summary>
  public Stations Stations = new();

  /// <summary>Train-aggregate physics (speed, distance, MR pressure, gradient).</summary>
  public Physics Physics = new();

  /// <summary>Driver-input state (notches, reverser, ATO/TASC/deadman).</summary>
  public Controllers Controllers = new();

  /// <summary>Door state: safe-to-proceed flag plus per-car open-side.</summary>
  public Doors Doors = new();

  /// <summary>Vocabulary-keyed panel-lamp values (0=off, 1=on, 2+=vehicle-specific).</summary>
  public Lamps Lamps = new();

  /// <summary>ATS pattern speed, class, and rich-state info.</summary>
  public Ats Ats = new();

  /// <summary>Upcoming signals with aspect, distance, and transponders.</summary>
  public Signals Signals = new();

  /// <summary>Current speed limit and the next change point.</summary>
  public SpeedLimit SpeedLimit = new();

  /// <summary>Per-car dynamic state (BC pressure, amperage, occupancy).</summary>
  public Cars Cars = new();

  /// <summary>Cab switch state (horn, buzzer, headlights, wiper).</summary>
  public Switches Switches = new();

  /// <summary>Outer game-state context (screen, crew role, drive mode, one-man).</summary>
  public GameState GameState = new();

  /// <summary>Sim/vendor-specific blocks keyed <c>"&lt;namespace&gt;:&lt;concern&gt;"</c>.</summary>
  public Dictionary<string, JsonElement>? Extensions;
}
