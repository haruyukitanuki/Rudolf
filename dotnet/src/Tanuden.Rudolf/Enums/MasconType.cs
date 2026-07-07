using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Cab master-controller (mascon) handle layout.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MasconType
{
  /// <summary>Single combined handle controlling both power and brake (ワンハンドル).</summary>
  OneHandle,

  /// <summary>Separate power and brake handles (ツーハンドル).</summary>
  TwoHandle
}
