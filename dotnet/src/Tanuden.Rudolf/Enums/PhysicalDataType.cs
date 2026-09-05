using System.Text.Json.Serialization;
using Tanuden.Rudolf.Sections;

namespace Tanuden.Rudolf.Enums;

/// <summary>
///   Availability of per-car and total data relating to a physical quantity.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PhysicalDataType
{
  /// <summary>No data is available.</summary>
  None,

  /// <summary>Only total data is available.</summary>
  TotalOnly,

  /// <summary>Per-car and total data is available.</summary>
  All,
}
