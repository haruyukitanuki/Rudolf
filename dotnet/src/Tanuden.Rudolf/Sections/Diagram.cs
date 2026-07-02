using Tanuden.Rudolf.Enums;

namespace Tanuden.Rudolf.Sections;

/// <summary>
///   Train-identity context for this run (train number, bound-for, service type, line direction).
///   All fields nullable: heuristic-derived fields (e.g. parsing the scenario title) MUST emit
///   null rather than guessing when the sim does not natively expose the value.
/// </summary>
public class Diagram
{
  /// <summary>Usually the same as Diagram Number.</summary>
  public string? TrainNumber;

  /// <summary>The last station on the Diagram.</summary>
  public string? BoundFor;

  /// <summary>Service Type.</summary>
  public string? ServiceType;

  /// <summary>Operation direct relative to the line.</summary>
  public LineDirection? Direction;

  /// <summary>Usually derived from Diagram Number using a formula dependent on the operator.</summary>
  public string? RunNumber;
}
