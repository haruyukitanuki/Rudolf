using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Reason for stop at a station.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StopType
{
  /// <summary>For standard revenue service, stop for passengers.</summary>
  PassengerStop,

  /// <summary>For operational reasons such as signal station turnback, parking, etc.</summary>
  OperationStop,

  /// <summary>Pass through without stopping</summary>
  Passing
}
