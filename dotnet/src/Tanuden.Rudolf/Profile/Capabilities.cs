using System.Collections.Generic;
using System.Text.Json;

namespace Tanuden.Rudolf.Profile;

/// <summary>
///   Capability flags keyed by dotted path matching <see cref="OutputDataFrame" /> field paths.
///   Values are usually booleans; some keys carry string-enum values (e.g. <c>physics.perCar</c>
///   is <c>"true" | "broadcast" | "unavailable"</c>, <c>ats.richState</c> is <c>"rich" | "eb-only" | "none"</c>).
///   Consumers read via <see cref="JsonElement" /> and call <c>.GetBoolean()</c>/<c>.GetString()</c> as needed.
/// </summary>
public class Capabilities : Dictionary<string, JsonElement>
{
}
