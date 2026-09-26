using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Direction relative to the line</summary>
[JsonConverter(typeof(Tanuden.Rudolf.Json.StringOnlyEnumConverter))]
public enum LineDirection
{
  /// <summary>Upbound</summary>
  Upbound,

  /// <summary>Downbound</summary>
  Downbound
}
