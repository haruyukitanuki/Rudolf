namespace Tanuden.Rudolf.Profile;

/// <summary>Scenario metadata</summary>
public class ScenarioInfo
{
  /// <summary>Scenario title.</summary>
  public string Title = string.Empty;

  /// <summary>Route identity (e.g. file path stem or route-pack name).</summary>
  public string Route = string.Empty;

  /// <summary>Scenario author (if the simulator exposes it). null otherwise.</summary>
  public string? Author;

  /// <summary>Scenario start time as ISO local datetime.</summary>
  public string ScenarioStartTime = "0000-01-01T00:00:00";

  /// <summary>
  ///   Train/diagram number when known at scenario load. Mirrors
  ///   <see cref="Tanuden.Rudolf.Sections.Diagram.TrainNumber" />.
  /// </summary>
  public string? DiagramNumber;

  /// <summary>Destination when known at scenario load. Mirrors <see cref="Tanuden.Rudolf.Sections.Diagram.BoundFor" />.</summary>
  public string? BoundFor;

  /// <summary>Service type when known at scenario load. Mirrors <see cref="Tanuden.Rudolf.Sections.Diagram.ServiceType" />.</summary>
  public string? ServiceType;
}
