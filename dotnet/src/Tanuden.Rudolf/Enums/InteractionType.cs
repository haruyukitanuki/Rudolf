using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Type of action with other trains.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InteractionType
{
  /// <summary>Passengers can change to a train that has already stopped at the station. (接)</summary>
  Connecting,

  /// <summary>Wait for a train to clear the tracks ahead. (交)</summary>
  ExchangeMovement,

  /// <summary>Passengers can change to a train that has not arrived yet. (連)</summary>
  Transfer,

  /// <summary>Wait for a train to pass from behind. (待)</summary>
  Wait,
}
