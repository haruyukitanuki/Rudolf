namespace Tanuden.Rudolf.Profile;

/// <summary>Identity of the simulator + adapter producing this <see cref="SimulatorProfile" />.</summary>
public class SimInfo
{
  /// <summary>Simulator software name</summary>
  public string Name = string.Empty;

  /// <summary>Simulator software version (if possible, in semver); null when the sim has no version.</summary>
  public string? Version = null;

  /// <summary>Adapter package name</summary>
  public string AdapterName = string.Empty;

  /// <summary>Adapter version string in semver</summary>
  public string AdapterVersion = string.Empty;
}
