using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>How the brake handle behaves on the master controller.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MasconBrakeType
{
  /// <summary>Discrete, self-lapping stepped brake notches (段階ブレーキ).</summary>
  Notched,

  /// <summary>Continuous brake with a lap position for holding pressure (直通ブレーキ, ラップ可能).</summary>
  LapCapable
}
