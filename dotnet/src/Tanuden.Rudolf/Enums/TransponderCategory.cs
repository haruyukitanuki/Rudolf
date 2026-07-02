using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>
///   Functional category of a track-side transponder/beacon.
///   Adapters MUST emit <c>null</c> rather than guess when they can't categorize.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransponderCategory
{
  /// <summary>Pattern source transponder.</summary>
  Pattern,

  /// <summary>Signal transponder</summary>
  Signal,

  /// <summary>TASC transponder</summary>
  TASC,

  /// <summary>Recognized but does not fit any specific category.</summary>
  Other
}
