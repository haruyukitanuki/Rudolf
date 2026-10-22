using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Air braking system used by the train. Affects validity of BP and SAP pressure values.</summary>
[JsonConverter(typeof(Tanuden.Rudolf.Json.StringOnlyEnumConverter))]
public enum PneumaticBrakeType
{
  /// <summary>Electric command brake. BP and SAP availability depend on the train.</summary>
  ElectricCommand,

  /// <summary>Increasing pressure of the straight air pipe applies the brakes. Has SAP but not BP.</summary>
  StraightAir,

  /// <summary>Decreasing pressure of the pressurized brake pipe applies the brakes. Has BP but not SAP.</summary>
  AutomaticAir,

  /// <summary>Other types of air brakes, or none are fitted.</summary>
  Other
}
