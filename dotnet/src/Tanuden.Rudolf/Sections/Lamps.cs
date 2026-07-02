using System.Collections.Generic;

namespace Tanuden.Rudolf.Sections;

/// <summary>
///   Panel-lamp state, vocabulary-keyed.
///   Values: <c>0</c> = off, <c>1</c> = on, <c>2+</c> = vehicle-specific alternative
///   (blinking, dim, multicolor). Basic TIMS that only knows 0/1 SHOULD treat any nonzero as truthy.
///   Default keys include <c>doorClose</c>, <c>atsReady</c>, <c>atsBrakeApply</c>, <c>atsOpen</c>,
///   <c>regenerative</c>, <c>ebTimer</c>, <c>emergencyBrake</c>, <c>overload</c>, <c>pilot</c>, <c>ato</c>.
///   Sim/vehicle-specific keys are allowed freely.
/// </summary>
public class Lamps
{
  /// <summary>Lamp key → integer state. See class-level docs for the value convention.</summary>
  public Dictionary<string, int> Values = new();
}
