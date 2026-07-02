using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Functional classification of a signal head.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SignalType
{
  /// <summary>Block signal (閉塞信号機).</summary>
  Block,

  /// <summary>Distant signal (遠方信号機).</summary>
  Distant,

  /// <summary>Call-on signal (誘導信号機).</summary>
  CallOn,

  /// <summary>Shunt signal (入換信号機).</summary>
  Shunt,

  /// <summary>Home signal (場内信号機).</summary>
  Home,

  /// <summary>Departure signal (出発信号機).</summary>
  Departure
}
