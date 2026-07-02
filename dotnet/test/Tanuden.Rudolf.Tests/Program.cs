using System.Text;
using System.Text.Json;
using Tanuden.Rudolf;
using Tanuden.Rudolf.Json;
using Tanuden.Rudolf.Sections;

// Regression harness for the "poison frame" bug: a non-finite double (BVE reports an unlimited speed
// limit as +Infinity) made RudolfJson serialization throw, permanently wedging the MMF writer.
// Run with `dotnet run`; exits non-zero if any check fails.

var failures = new List<string>();

void Check(string name, bool ok, string? detail = null)
{
    Console.WriteLine($"{(ok ? "PASS" : "FAIL")}  {name}{(detail is null ? "" : $"  -> {detail}")}");
    if (!ok) failures.Add(name);
}

// Reproduction: a frame carrying non-finite speed limits (the exact shape that crashed WriteFrame).
var poison = new OutputDataFrame
{
    SpeedLimit = new SpeedLimit
    {
        Current = double.PositiveInfinity,
        Next = new List<SpeedLimitNext> { new() { Limit = double.NegativeInfinity, Distance = 1234 } },
    },
};

byte[]? bytes = null;
try
{
    bytes = JsonSerializer.SerializeToUtf8Bytes(poison, RudolfJson.Options);
    Check("non-finite frame serializes without throwing", true);
}
catch (Exception ex)
{
    Check("non-finite frame serializes without throwing", false, $"{ex.GetType().Name}: {ex.Message}");
}

if (bytes is not null)
{
    var json = Encoding.UTF8.GetString(bytes);
    Check("output has no 'Infinity' token", !json.Contains("Infinity"));
    Check("output has no 'NaN' token", !json.Contains("NaN"));

    try
    {
        var back = JsonSerializer.Deserialize<OutputDataFrame>(bytes, RudolfJson.Options);
        Check("sanitized frame round-trips into the model",
            back is not null && double.IsFinite(back.SpeedLimit.Current));
    }
    catch (Exception ex)
    {
        Check("sanitized frame round-trips into the model", false, ex.Message);
    }
}

// Finite values must serialize identically (near-zero blast radius for the converter).
var normal = new OutputDataFrame { SpeedLimit = new SpeedLimit { Current = 70.5 } };
var normalJson = Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(normal, RudolfJson.Options));
Check("finite value preserved verbatim", normalJson.Contains("70.5"));

// --- Field-order regression: the wire JSON must follow the spec/class declaration order, never
//     alphabetical. Default instances are used so section collections are empty and each checked key
//     appears exactly once, making IndexOf position comparisons unambiguous. ---
static bool InOrder(string json, params string[] keys)
{
    var last = -1;
    foreach (var k in keys)
    {
        var at = json.IndexOf($"\"{k}\"", StringComparison.Ordinal);
        if (at < 0 || at <= last) return false;
        last = at;
    }

    return true;
}

var frameJson = Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(new OutputDataFrame(), RudolfJson.Options));
Check("frame starts with schemaVersion", frameJson.StartsWith("{\"schemaVersion\":", StringComparison.Ordinal),
    frameJson[..Math.Min(40, frameJson.Length)]);
Check("frame envelope in spec order (schemaVersion, kind, scenarioId, sentAt, time)",
    InOrder(frameJson, "schemaVersion", "kind", "scenarioId", "sentAt", "time"));
Check("physics in spec order (speed, fromStartDistance, absoluteDistance, gradient, mrPressure)",
    InOrder(frameJson, "speed", "fromStartDistance", "absoluteDistance", "gradient", "mrPressure"));

var profileJson = Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(new SimulatorProfile(), RudolfJson.Options));
Check("profile starts with schemaVersion", profileJson.StartsWith("{\"schemaVersion\":", StringComparison.Ordinal),
    profileJson[..Math.Min(40, profileJson.Length)]);
Check("profile: sim before capabilities (not alphabetical)", InOrder(profileJson, "sim", "capabilities"));

Console.WriteLine();
if (failures.Count == 0)
{
    Console.WriteLine("ALL TESTS PASSED");
    return 0;
}

Console.WriteLine($"{failures.Count} TEST(S) FAILED");
return 1;
