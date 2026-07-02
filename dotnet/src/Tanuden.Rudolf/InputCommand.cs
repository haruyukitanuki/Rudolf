using Tanuden.Rudolf.Input;

namespace Tanuden.Rudolf;

/// <summary>
///   Single driver-input event sent from a consumer back into the simulator (consumer → sim).
///   Carries one <see cref="Command" /> payload (a <c>SetXxxCommand</c> subtype) plus a
///   <see cref="SequenceNumber" /> for ordering and idempotency.
/// </summary>
public class InputCommand
{
  /// <summary>RUDOLF schema version this command conforms to.</summary>
  public string SchemaVersion = Constants.SchemaVersion;

  /// <summary>Document discriminator. Always <c>"InputCommand"</c>.</summary>
  public string Kind = "InputCommand";

  /// <summary>Opaque identifier tying all documents of one play-session together.</summary>
  public string ScenarioId = string.Empty;

  /// <summary>ISO 8601 timestamp at the producer.</summary>
  public string SentAt = string.Empty;

  /// <summary>Monotonic per consumer; for ordering/idempotency.</summary>
  public long SequenceNumber;

  /// <summary>The command payload. Discriminated on its <c>kind</c> string at the wire level.</summary>
  public Command Command = default!;
}
