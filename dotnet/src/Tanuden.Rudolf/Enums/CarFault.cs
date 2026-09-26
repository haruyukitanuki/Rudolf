using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Fault of body/roof/cab-mounted equipment on a single car (<c>OutputDataFrame.cars.list[...].faults</c>). Empty = normal; null = not modeled.</summary>
[JsonConverter(typeof(Tanuden.Rudolf.Json.StringOnlyEnumConverter))]
public enum CarFault
{
  /// <summary>Doors (ドア故障).</summary>
  Door,

  /// <summary>Pantograph (パンタグラフ異常), including persistent dewirement (離線).</summary>
  Pantograph,

  /// <summary>Traction equipment reported at car level (主回路関連の故障). Fine-grained reporting uses <see cref="BogieFault.Traction" />.</summary>
  Traction,

  /// <summary>Brake control equipment (ブレーキ装置異常): BCU, sticking.</summary>
  Brake,

  /// <summary>Air compressor (空気圧縮機異常, CP).</summary>
  Compressor,

  /// <summary>Auxiliary power supply (補助電源装置異常, SIV).</summary>
  AuxiliaryPower,

  /// <summary>Onboard safety device (保安装置異常): ATS/ATC equipment.</summary>
  SafetyDevice,

  /// <summary>Monitor system (モニタ装置異常).</summary>
  Monitor,

  /// <summary>Train radio (無線異常).</summary>
  TrainRadio,

  /// <summary>Air conditioning (空調装置異常). Comfort-only.</summary>
  AirConditioner,

  /// <summary>Unclassified or sim-specific (その他).</summary>
  Other
}
