using System.Collections.Generic;
using System.Text.Json;
using Tanuden.Rudolf.Enums;

namespace Tanuden.Rudolf.Profile;

/// <summary>
///   Capability flags keyed by dotted path matching <see cref="OutputDataFrame" /> field paths.
///   Values are usually booleans; some keys carry string-enum values (e.g. <c>physics.perCar</c>
///   is <c>"true" | "broadcast" | "unavailable"</c>).
///   Consumers read via <see cref="JsonElement" /> and call <c>.GetBoolean()</c>/<c>.GetString()</c> as needed.
/// </summary>
public class Capabilities : Dictionary<string, JsonElement>
{
  /// <summary>
  ///   Create a new empty Capabilities section.
  /// </summary>
  public Capabilities()
  {
    /*
    this.Add("time.dateKnown", JsonSerializer.SerializeToElement(false));
    this.Add("physics.gradient", JsonSerializer.SerializeToElement(false));
    this.Add("physics.curveRadius", JsonSerializer.SerializeToElement(false));
    this.Add("physics.perCar", JsonSerializer.SerializeToElement(PerCarPhysicsType.Unavailable));
    this.Add("ats.richState", JsonSerializer.SerializeToElement(false));
    this.Add("stations.next", JsonSerializer.SerializeToElement(NextItemArrayType.None));
    this.Add("speedLimits.next", JsonSerializer.SerializeToElement(NextItemArrayType.None));
    this.Add("signals.next", JsonSerializer.SerializeToElement(NextItemArrayType.None));

    this.Add("input.command.setNotch", JsonSerializer.SerializeToElement(false));
    this.Add("input.command.setPowerNotch", JsonSerializer.SerializeToElement(false));
    this.Add("input.command.setBrakeNotch", JsonSerializer.SerializeToElement(false));
    this.Add("input.command.setBrakeSAP", JsonSerializer.SerializeToElement(false));
    this.Add("input.command.setReverser", JsonSerializer.SerializeToElement(false));
    this.Add("input.command.setButton", JsonSerializer.SerializeToElement(false));
    this.Add("input.command.setWiper", JsonSerializer.SerializeToElement(false));
    this.Add("input.command.setAtoNotch", JsonSerializer.SerializeToElement(false));
    this.Add("input.command.setDeadman", JsonSerializer.SerializeToElement(false));
    */
  }
}
