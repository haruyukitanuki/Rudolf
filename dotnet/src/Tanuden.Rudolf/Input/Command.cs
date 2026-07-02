using System.Text.Json.Serialization;
using Tanuden.Rudolf.Json;

namespace Tanuden.Rudolf.Input;

/// <summary>
///   Base type for the discriminated-union <c>command</c> payload on an <see cref="InputCommand" /> document.
///   Serialization writes a <c>kind</c> discriminator string; deserialization dispatches on it.
///   Pattern-match the concrete subtype (<c>SetNotchCommand</c>, <c>SetButtonCommand</c>, ...) to read fields.
/// </summary>
[JsonConverter(typeof(CommandJsonConverter))]
public abstract class Command
{
}
