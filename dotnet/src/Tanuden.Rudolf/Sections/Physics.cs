namespace Tanuden.Rudolf.Sections;

/// <summary>Train-aggregate physical state. Per-car values live in <see cref="Cars" />.</summary>
public class Physics
{
  /// <summary>km/h; train-level; always present.</summary>
  public double Speed;

  /// <summary>Meters traveled from scenario start; always present.</summary>
  public double FromStartDistance;

  /// <summary>Absolute kilometer-post; null when sim doesn't expose chainage.</summary>
  public double? AbsoluteDistance;

  /// <summary>Per mille; null when the sim doesn't expose gradient.</summary>
  public double? Gradient;

  /// <summary>MR Pressure in kPa; train-level; always present.</summary>
  public double MrPressure;
}
