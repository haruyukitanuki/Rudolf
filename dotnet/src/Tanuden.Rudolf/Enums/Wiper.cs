using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Windshield wiper mode.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Wiper
{
  /// <summary>Wipers off.</summary>
  Off,

  /// <summary>Intermittent wipe (間欠).</summary>
  Intermittent,

  /// <summary>Continuous low-speed wipe.</summary>
  Low,

  /// <summary>Continuous high-speed wipe.</summary>
  High
}
