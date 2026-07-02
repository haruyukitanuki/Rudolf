using System.Collections.Generic;

namespace Tanuden.Rudolf.Sections;

/// <summary>Per-car dynamic state. Static composition lives in <see cref="Tanuden.Rudolf.Profile.VehicleInfo.Cars" />.</summary>
public class Cars
{
  /// <summary>
  ///   One entry per car in left-to-right display order.
  ///   The first car in the list doesn't always represent lead.
  ///   Empty when the sim doesn't expose per-car detail.
  /// </summary>
  public List<Car> List = new();
}

/// <summary>Dynamic state for a single car.</summary>
public class Car
{
  /// <summary> Matches <see cref="Tanuden.Rudolf.Profile.CarStaticInfo.CarNo" />.</summary>
  public int CarNo;

  /// <summary>kPa; null when sim doesn't expose per-car BC pressure.</summary>
  public double? BcPressure;

  /// <summary>Amperes; null when sim doesn't expose per-car motor current.</summary>
  public double? Amperage;

  /// <summary>Percentage filled (May exceed 100%).</summary>
  public double? OccupancyRate;
}
