using System.Collections.Generic;
using Tanuden.Rudolf.Enums;

namespace Tanuden.Rudolf.Profile;

/// <summary>Vehicle identity plus static per-car composition (cabs, motors, pantographs).</summary>
public class VehicleInfo
{
  /// <summary>
  ///   Human display name for the formation (e.g. <c>"225系0番台"</c>). Please ensure the correct kanji is used
  ///   for kei and bandai.
  /// </summary>
  public string Name = string.Empty;

  /// <summary>
  ///   Vehicle/formation model identifier (e.g. <c>"E233-1000"</c>). For maximum interoperability, it should
  ///   be in format of <c>series-subseries</c>; Avoid using Japanese characters.
  /// </summary>
  public string Model = string.Empty;

  /// <summary>
  ///   Operating company (e.g. <c>"EastJapanRailwayCompany"</c>, <c>"TokyuCorporation"</c>).
  ///   For maximum compatibility, refer to
  ///   Japanese Wikipedia for the full operator name (not group) and TitleCase it.
  /// </summary>
  public string Operator = string.Empty;

  /// <summary>
  ///   One entry per car. List is displayed from left to right.
  ///   The first car in the list doesn't always represent lead.
  /// </summary>
  public List<CarStaticInfo> Cars = new();

  /// <summary>
  ///   Car no. for the lead car. This should usually be the car number of either the first or last item in
  ///   <see cref="VehicleInfo.Cars" />.
  /// </summary>
  public int LeadCar = 0;

  /// <summary>
  ///   Static control-hardware description (mascon layout, notch counts, holding brake, compressor
  ///   pressures). Inner fields are null when the sim has no value for them.
  /// </summary>
  public VehicleCapabilities Capabilities = new();
}

/// <summary>Static composition for a single car (cabs, motors, pantograph layout).</summary>
public class CarStaticInfo
{
  /// <summary>Matches <see cref="Tanuden.Rudolf.Sections.Car.CarNo" />.</summary>
  public int CarNo;

  /// <summary>
  ///   Per-car model code (e.g. <c>"KuHaE233"</c>, <c>MoHa225-51xx</c>). For maximum interoperability, romanise all
  ///   kana in TitleCase.
  /// </summary>
  public string Model = string.Empty;

  /// <summary>True when this car has a driver's cab.</summary>
  public bool HasDriverCab;

  /// <summary>True when this car has a conductor's cab.</summary>
  public bool HasConductorCab;

  /// <summary>True when this car is motorized (M/MM').</summary>
  public bool HasMotor;

  /// <summary>True when this car carries one or more pantographs.</summary>
  public bool HasPantograph;

  /// <summary>Which way this car's driver cab faces; null when the car has no driver cab.</summary>
  public Direction? CabDirection;

  /// <summary>Style of pantograph; null when <see cref="HasPantograph" /> is false.</summary>
  public PantographType? PantographType;

  /// <summary>Which end(s) the pantograph(s) lean toward; null when <see cref="HasPantograph" /> is false or unknown.</summary>
  public PantographDirection? PantographDirection;

  /// <summary>Car length in meters. -1 if unknown.</summary>
  public double Length = -1;
}
