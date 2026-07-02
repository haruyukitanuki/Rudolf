using Tanuden.Rudolf.Enums;

namespace Tanuden.Rudolf.Sections;

/// <summary>Driver-input state: handles, reverser, and automatic driving systems.</summary>
public class Controllers
{
  /// <summary>Current power notch.</summary>
  public int PowerNotch;

  /// <summary>Current brake notch.</summary>
  public int BrakeNotch;

  /// <summary>Reverser position. See <see cref="Tanuden.Rudolf.Enums.Reverser" />.</summary>
  public Reverser Reverser;

  /// <summary>Auto Train Operation state; null when the vehicle has no ATO.</summary>
  public AtoState? Ato;

  /// <summary>Train Automatic Stop Controller state; null when the vehicle has no TASC.</summary>
  public TascState? Tasc;

  /// <summary>Which deadman/EB channel is currently engaged; null when nothing is engaged or the sim doesn't model deadman.</summary>
  public EBDeadmanMethod? Deadman;
}

/// <summary>ATO state.</summary>
public class AtoState
{
  /// <summary>True when ATO is engaged and currently driving.</summary>
  public bool Active;

  /// <summary>The notch ATO is currently commanding; null when ATO is not asserting a value.</summary>
  public int? Notch;
}

/// <summary>Train Automatic Stop Controller state.</summary>
public class TascState
{
  /// <summary>True when TASC is engaged.</summary>
  public bool Active;

  /// <summary>The brake notch TASC is currently commanding; null when not asserting.</summary>
  public int? Notch;

  /// <summary>True during the final low-speed alignment phase against the platform stop marker.</summary>
  public bool Inching;
}
