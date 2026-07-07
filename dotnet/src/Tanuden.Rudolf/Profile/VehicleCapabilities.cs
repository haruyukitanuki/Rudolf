using Tanuden.Rudolf.Enums;

namespace Tanuden.Rudolf.Profile;

/// <summary>
///   Static control-hardware description for the vehicle (mascon layout, notch counts, holding brake,
///   air-compressor governor pressures). Distinct from <see cref="SimulatorProfile.Capabilities" />, which
///   declares which live <see cref="OutputDataFrame" /> fields the adapter populates. Every field is
///   nullable: <c>null</c> means the sim has no value for it right now.
/// </summary>
public class VehicleCapabilities
{
  /// <summary>Master-controller handle layout; null when unknown.</summary>
  public MasconType? MasconType;

  /// <summary>Brake-handle behaviour; null when unknown.</summary>
  public MasconBrakeType? MasconBrakeType;

  /// <summary>Number of power notches (e.g. P1..P5 = 5); null when unknown.</summary>
  public int? PowerNotches;

  /// <summary>Number of service brake notches (e.g. B1..B7 = 7); null when unknown.</summary>
  public int? BrakeNotches;

  /// <summary>
  ///   Signed notch value representing EB in the SetNotch encoding (e.g. <c>-8</c>); null when unknown.
  /// </summary>
  public int? EbNotch;

  /// <summary>
  ///   Number of holding-brake (抑速) notches; <c>0</c> when the vehicle has none, null when unknown.
  /// </summary>
  public int? HoldingBrakeNotches;

  /// <summary>Air-compressor cut-in (start) pressure in kPa; null when unknown.</summary>
  public double? CpStartPressure;

  /// <summary>Air-compressor cut-out (stop) pressure in kPa; null when unknown.</summary>
  public double? CpStopPressure;
}
