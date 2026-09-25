using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Type of driving run</summary>
[JsonConverter(typeof(Tanuden.Rudolf.Json.StringOnlyEnumConverter))]
public enum DriveMode
{
  /// <summary>Scored/graded run (e.g. scenario mode)</summary>
  Scored,

  /// <summary>For non-scored runs (e.g. free drive)</summary>
  Unscored,

  /// <summary>Some other mode the schema doesn't name explicitly</summary>
  Other
}
