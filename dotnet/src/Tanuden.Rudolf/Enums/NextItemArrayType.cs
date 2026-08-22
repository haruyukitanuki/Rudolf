using System.Text.Json.Serialization;
using Tanuden.Rudolf.Sections;

namespace Tanuden.Rudolf.Enums;

/// <summary>
///   Format of arrays used to store data in <see cref="Stations"/>, <see cref="SpeedLimits"/>, and <see cref="Signals"/>.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NextItemArrayType
{
  /// <summary>No items in front of the train are ever exposed.</summary>
  None,

  /// <summary>0 or 1 items in front of the train can be exposed.</summary>
  Single,

  /// <summary>Any number of items in front of the train can be exposed, not necessarily to the end of the scenario.</summary>
  MultiDynamic,

  /// <summary>
  ///   All items from the start to the end of the scenario are exposed.
  ///   Only applicable to <see cref="Stations.List"/>.
  /// </summary>.
  MultiStatic
}
