using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Mechanical style of a pantograph collector.</summary>
[JsonConverter(typeof(Tanuden.Rudolf.Json.StringOnlyEnumConverter))]
public enum PantographType
{
  /// <summary>Single-arm</summary>
  SingleArm,

  /// <summary>Scissor/diamond</summary>
  Scissor
}
