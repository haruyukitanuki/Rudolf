using System.Text.Json.Serialization;
using Tanuden.Rudolf.Sections;

namespace Tanuden.Rudolf.Enums;

/// <summary>
///   Availability of data in arrays in <see cref="Cars"/>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PerCarPhysicsType
{
  /// <summary>Data is present for all cars.</summary>
  True,

  /// <summary>Data is present only for the first car. Consumers must broadcast from the first index of the arrays.</summary>
  Broadcast,

  /// <summary>No per-car data is available.</summary>
  Unavailable
}
