using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Direction relative to the line</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LineDirection
{
  /// <summary>Upbound</summary>
  Upbound,

  /// <summary>Downbound</summary>
  Downbound
}
