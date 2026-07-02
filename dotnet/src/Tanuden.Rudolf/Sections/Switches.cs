using Tanuden.Rudolf.Enums;

namespace Tanuden.Rudolf.Sections;

/// <summary>Cab-switch state: horn, buzzer, headlights, wiper.</summary>
public class Switches
{
  /// <summary>True while the air horn is sounding.</summary>
  public bool HornAir;

  /// <summary>True while the electric horn is sounding.</summary>
  public bool HornElectric;

  /// <summary>Driver-initiated buzzer (sends to conductor).</summary>
  public bool BuzzerDriver;

  /// <summary>Conductor-initiated buzzer (sends to driver).</summary>
  public bool BuzzerConductor;

  /// <summary>True when headlights are on. Use <see cref="HighBeam" /> to distinguish low/high beam.</summary>
  public bool Headlights;

  /// <summary>True when the headlights are on high beam. Only meaningful when <see cref="Headlights" /> is true.</summary>
  public bool HighBeam;

  /// <summary>Wiper mode; null when the vehicle/sim has no wiper.</summary>
  public Wiper? Wiper;
}
