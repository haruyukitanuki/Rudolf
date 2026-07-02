using System.Collections.Generic;
using Tanuden.Rudolf.Enums;

namespace Tanuden.Rudolf.Sections;

/// <summary>Upcoming signals visible ahead of the train, each with its current aspect and transponders.</summary>
public class Signals
{
  /// <summary>Signals in distance order (nearest first). Empty when no signals are visible/known.</summary>
  public List<Signal> List = new();
}

/// <summary>A single upcoming signal.</summary>
public class Signal
{
  /// <summary>Sim-native signal name when available; some sims synthesize from block distance (e.g. <c>"SecXXXm"</c>).</summary>
  public string? Name;

  /// <summary>Functional class of the signal (block, distant, call-on, shunt, home, departure).</summary>
  public SignalType? Type;

  /// <summary>Signal aspect index. See <see cref="SignalPhase" /> for the default vocabulary.</summary>
  public int? Phase;

  /// <summary>Meters ahead.</summary>
  public double Distance;

  /// <summary>Beacons/transponders associated with this signal block.</summary>
  public List<Transponder> Transponders = new();
}

/// <summary>A single beacon/transponder placed in the trackbed.</summary>
public class Transponder
{
  /// <summary>Functional category (ATS pattern, ATC, speed limit, stop marker, ...); null when unclassified.</summary>
  public TransponderCategory? Category;

  /// <summary>Sim-native beacon type code; null when the sim doesn't expose it.</summary>
  public int? Code;

  /// <summary>km/h imposed by this transponder, if applicable.</summary>
  public double? SpeedLimit;

  /// <summary>Meters; negative = already passed.</summary>
  public double Distance;
}
