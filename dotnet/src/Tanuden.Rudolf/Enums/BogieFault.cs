using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Fault of bogie-mounted equipment (motors, axles, brakes, collector shoe).</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BogieFault
{
  /// <summary>Traction motors mounted on this bogie (主電動機).</summary>
  Traction,

  /// <summary>Brake equipment mounted on this bogie (台車ブレーキ): dragging/stuck brake, rigging fault. Control-side faults use <see cref="CarFault.Brake" />.</summary>
  Brake,

  /// <summary>Collector shoe sheered/damaged (集電靴の破損・脱落); third-rail vehicles.</summary>
  CollectorShoe,

  /// <summary>Unclassified or sim-specific (その他).</summary>
  Other
}
