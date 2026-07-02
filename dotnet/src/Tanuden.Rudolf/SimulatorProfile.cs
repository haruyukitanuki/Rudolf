using Tanuden.Rudolf.Profile;

namespace Tanuden.Rudolf;

/// <summary>
///   Once-per-scenario descriptor of what the sim+vehicle can and cannot fill in subsequent
///   <see cref="OutputDataFrame" /> documents. Sent once at scenario load (and again on vehicle change),
///   cached by <see cref="ScenarioId" />. Lets consumers render "N/A" instead of misleading zeros
///   for fields the sim cannot source.
/// </summary>
public class SimulatorProfile
{
  /// <summary>RUDOLF schema version this document conforms to.</summary>
  public string SchemaVersion = Constants.SchemaVersion;

  /// <summary>Document discriminator. Always <c>"SimulatorProfile"</c>.</summary>
  public string Kind = "SimulatorProfile";

  /// <summary>Opaque identifier tying all documents of one play-session together.</summary>
  public string ScenarioId = string.Empty;

  /// <summary>ISO 8601 timestamp at the producer.</summary>
  public string SentAt = string.Empty;

  /// <summary>Simulator + adapter identity (name + version).</summary>
  public SimInfo Sim = new();

  /// <summary>Scenario metadata (title, route, author, start time, diagram info).</summary>
  public ScenarioInfo Scenario = new();

  /// <summary>Vehicle identity plus static per-car composition (cabs, motors, pantographs).</summary>
  public VehicleInfo Vehicle = new();

  /// <summary>Capability map declaring which <see cref="OutputDataFrame" /> fields are reliably populated.</summary>
  public Capabilities Capabilities = new();

  /// <summary>Vocabulary overrides (lamps, signal phase, transponder codes) for this sim+vehicle.</summary>
  public Vocabularies Vocabularies = new();
}
