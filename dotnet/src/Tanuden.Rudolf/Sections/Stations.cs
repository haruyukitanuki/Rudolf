using System.Collections.Generic;
using Tanuden.Rudolf.Enums;

namespace Tanuden.Rudolf.Sections;

/// <summary>Ordered station list for the diagram plus pointers into it.</summary>
public class Stations
{
  /// <summary>Stations in scheduled visit order. May be empty before the diagram is loaded.</summary>
  public List<Station> List = new();

  /// <summary>Index into <see cref="List" /> for the station the train is currently at; null when between stations.</summary>
  public int? CurrentIndex;

  /// <summary>Index into <see cref="List" /> for the next station ahead; null at terminus.</summary>
  public int? NextIndex;
}

/// <summary>A single station entry within <see cref="Stations.List" />.</summary>
public class Station
{
  /// <summary>Position of this station within <see cref="Stations.List" />. Useful for round-tripping.</summary>
  public int Index;

  /// <summary>
  ///   Station display name only (typically Japanese, e.g. <c>"新宿"</c>) — no station codes or
  ///   numbering (e.g. <c>"品川"</c>, never <c>"KK01 品川"</c> / <c>"品川(JK20)"</c> / <c>"KK01"</c>).
  ///   Emitted as literal UTF-8, with no <c>\u</c> escape sequences.
  /// </summary>
  public string Name = string.Empty;

  /// <summary>Meters from scenario start; always present.</summary>
  public double FromStartDistance;

  /// <summary>Absolute kilometer-post; null when the sim doesn't expose chainage.</summary>
  public double? AbsoluteDistance;

  /// <summary>Which side the doors open. See <see cref="Tanuden.Rudolf.Enums.DoorSide" /> for the int convention.</summary>
  public int DoorSide;

  /// <summary>Whether this is a passenger stop, operation-only stop, or pass.</summary>
  public StopType? StopType;

  /// <summary>ISO datetime; null for first station and passing-only stops.</summary>
  public string? Arrival;

  /// <summary>ISO datetime; null for last station.</summary>
  public string? Departure;

  /// <summary>Platform/stop position name; null when not specified.</summary>
  public string? StopPositionName;

  /// <summary>True when this station is a timing point (採時駅); null when the sim doesn't model it.</summary>
  public bool? IsTimeTaken;

  /// <summary>
  ///   Candidate stop-position markers as car-counts for this station's current direction/platform (e.g.
  ///   <c>[3, 4, 6]</c>); null when unknown.
  /// </summary>
  public int[]? StopPositions;
}
