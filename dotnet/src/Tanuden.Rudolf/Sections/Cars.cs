using System.Collections.Generic;
using Tanuden.Rudolf.Enums;

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

/// <summary>Dynamic state of one bogie. Index-aligned with <see cref="Tanuden.Rudolf.Profile.CarStaticInfo.Bogies" />.</summary>
public class BogieDynamic
{
  /// <summary>Matches the static entry at the same index.</summary>
  public BogiePosition Position;

  /// <summary>kPa; null when sim doesn't expose per-bogie BC pressure.</summary>
  public double? BcPressure;

  /// <summary>Amperes; null on unpowered bogies or when sim doesn't expose per-bogie motor current.</summary>
  public double? Amperage;

  /// <summary>Bogie-scoped faults; empty = normal; null = not modeled.</summary>
  public List<BogieFault>? Faults;
}

/// <summary>Dynamic state for a single car.</summary>
public class Car
{
  /// <summary> Matches <see cref="Tanuden.Rudolf.Profile.CarStaticInfo.CarNo" />.</summary>
  public int CarNo;

  /// <summary>Percentage filled (May exceed 100%).</summary>
  public double? OccupancyRate;

  /// <summary>Load in kg.</summary>
  public double? LoadMass;

  /// <summary>Body/roof/cab-scoped faults. Empty = normal; null = not modeled.</summary>
  public List<CarFault>? Faults;

  /// <summary>
  ///   Per-bogie dynamic state, index-aligned with <see cref="Tanuden.Rudolf.Profile.CarStaticInfo.Bogies" />.
  ///   Null when the sim doesn't model per-bogie data (<c>cars.bogies</c> capability absent/false).
  ///   Replaces the former car-level <c>BcPressure</c> and <c>Amperage</c>.
  /// </summary>
  public List<BogieDynamic>? Bogies;
}
