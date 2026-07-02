using System.Collections.Generic;

namespace Tanuden.Rudolf.Sections;

/// <summary>Door state: a load-bearing safe-to-proceed flag plus per-car open-side detail.</summary>
public class Doors
{
  /// <summary>Pilot lamp.</summary>
  public bool AllClosed = true;

  /// <summary>Per-car door state. Empty when the sim doesn't expose per-car detail.</summary>
  public List<CarDoorState> PerCar = new();
}

/// <summary>Door state for a single car.</summary>
public class CarDoorState
{
  /// <summary>Matches <see cref="Tanuden.Rudolf.Profile.CarStaticInfo.CarNo" />.</summary>
  public int CarNo;

  /// <summary>
  ///   Which side(s) are open. See <see cref="Tanuden.Rudolf.Enums.SideOpened" /> for the int convention.
  ///   <c>null</c> if doors opened but side unknown
  /// </summary>
  public int? SideOpened;
}
