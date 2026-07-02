using System.Collections.Generic;
using Tanuden.Rudolf.Enums;

namespace Tanuden.Rudolf.Sections;

/// <summary>Currently enforced speed limit plus the upcoming change points.</summary>
public class SpeedLimit
{
  /// <summary>Current speed limit in km/h. <c>-1</c> means no posted limit (an unlimited section).</summary>
  public double Current;

  /// <summary>Origin of the current limit (posted, signal-imposed, or restriction); null when unknown.</summary>
  public SpeedLimitType? CurrentType;

  /// <summary>
  ///   Upcoming speed-limit changes ahead of the train, ordered nearest-first (ascending
  ///   <see cref="SpeedLimitNext.Distance" />), so <c>Next[0]</c> is the closest change. Producers that
  ///   only know the immediate next change emit a single-element list; producers that know the full
  ///   forward sequence emit them all (see <c>capabilities['speedLimit.next']</c>). <c>null</c> when no
  ///   upcoming change is known.
  /// </summary>
  public List<SpeedLimitNext>? Next;
}

/// <summary>The next speed-limit change ahead of the train.</summary>
public class SpeedLimitNext
{
  /// <summary>Next speed limit in km/h. <c>-1</c> means no posted limit (an unlimited section).</summary>
  public double Limit;

  /// <summary>Next speed limit section in meters.</summary>
  public double Distance;

  /// <summary>Type of speed limit.</summary>
  public SpeedLimitType? Type;
}
