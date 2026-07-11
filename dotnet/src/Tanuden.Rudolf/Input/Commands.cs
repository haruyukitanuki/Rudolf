using Tanuden.Rudolf.Enums;

namespace Tanuden.Rudolf.Input;

/// <summary>Set the combined power/brake notch (single-handle vehicles).</summary>
public class SetNotchCommand : Command
{
  /// <summary>Reserved emergency sentinel; any <see cref="Value" /> &lt;= <c>EB</c> is treated as Emergency regardless of <see cref="Relative" />.</summary>
  public const int EB = int.MinValue;

  /// <summary>
  ///   When <see cref="Relative" /> is false (absolute): signed combined notch: 0=N, +n=Pn,
  ///   -1=抑速, -2..=B1... When <see cref="Relative" /> is true: a signed step delta.
  ///   Any value &lt;= <see cref="EB" /> is Emergency regardless of <see cref="Relative" />.
  /// </summary>
  public int Value;

  /// <summary>When true, <see cref="Value" /> is a signed step delta instead of an absolute notch; default false.</summary>
  public bool Relative;
}

/// <summary>Set the power notch (two-handle vehicles).</summary>
public class SetPowerNotchCommand : Command
{
  /// <summary>0 = neutral; positive integers are P1, P2, ... up to the vehicle's max.</summary>
  public int Value;
}

/// <summary>Set the brake notch (two-handle vehicles).</summary>
public class SetBrakeNotchCommand : Command
{
  /// <summary>0 = release; positive integers are B1, B2, ... up to EB.</summary>
  public int Value;
}

/// <summary>Set the Service Application Pressure on a self-lapping brake handle (SAP/自弁).</summary>
public class SetBrakeSAPCommand : Command
{
  /// <summary>kPa; 0-400 = service, 410 = emergency.</summary>
  public double KPa;
}

/// <summary>Set the reverser position.</summary>
public class SetReverserCommand : Command
{
  /// <summary>Target reverser position.</summary>
  public Reverser Value;
}

/// <summary>Press, release, or toggle a discrete cab button.</summary>
public class SetButtonCommand : Command
{
  /// <summary>Which button: a <see cref="VehicleAction" /> or <see cref="GameAction" /> name, or a custom action string.</summary>
  public string Action = string.Empty;

  /// <summary>True = pressed/on; false = released/off.</summary>
  public bool State;
}

/// <summary>Set the wiper mode.</summary>
public class SetWiperCommand : Command
{
  /// <summary>Target wiper mode.</summary>
  public Wiper State;
}

/// <summary>Set the notch requested by an external ATO controller.</summary>
public class SetAtoNotchCommand : Command
{
  /// <summary>Signed combined notch in the same encoding as <see cref="SetNotchCommand.Value" />.</summary>
  public int Value;
}

/// <summary>Hold or release a deadman/EB channel.</summary>
public class SetDeadmanCommand : Command
{
  /// <summary>True while the channel is held; false when released.</summary>
  public bool Holding;

  /// <summary>Which channel (hand/foot).</summary>
  public EBDeadmanMethod Method;
}
