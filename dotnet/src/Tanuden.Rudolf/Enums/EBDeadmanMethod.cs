using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Which deadman/EB channel a <c>SetDeadman</c> command targets.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
// ReSharper disable once InconsistentNaming
public enum EBDeadmanMethod
{
  /// <summary>Hand (e.g. T-shaped mascons)</summary>
  Hand,

  /// <summary>Foot (e.g. older two-handed mascons)</summary>
  Foot,

  /// <summary>EB (usually on modern EMUs with single handle mascon)</summary>
  // ReSharper disable once InconsistentNaming
  EB
}
