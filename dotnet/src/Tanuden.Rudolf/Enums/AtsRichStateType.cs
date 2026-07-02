using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Machine-readable category of an ATS event in <see cref="Tanuden.Rudolf.Sections.AtsRichState" />.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AtsRichStateType
{
  /// <summary>
  ///   Flat continuous ceiling speed check (速度照査); no falling pattern active. Default cruising state when a fixed
  ///   speed limit is being enforced.
  /// </summary>
  SpeedCheck,

  /// <summary>Falling pattern enforced on a restrictive or stop signal: block, home, or departure (信号パターン).</summary>
  SignalP,

  /// <summary>Falling pattern enforced on a curve or turnout speed restriction (C信号).</summary>
  CurveP,

  /// <summary>Falling pattern protecting a track end or overrun-sensitive siding entry (終端パターン).</summary>
  TerminalP,

  /// <summary>Warning that a falling pattern is being approached (P接近).</summary>
  PApproach,

  /// <summary>Chime sounding, awaiting driver confirmation before EB applies; common for ATS-S (確認扱い).</summary>
  AckPending,

  /// <summary>Service brake applied by the system (常用ブレーキ動作).</summary>
  BApplication,

  /// <summary>Emergency brake applied by the system (非常ブレーキ動作).</summary>
  EbApplication,

  /// <summary>Falling pattern preventing station overruns and accidental passes (停車パターン・停通防止).</summary>
  StopP,

  /// <summary>Traction power cut off by the system (ノッチカット).</summary>
  NotchCut,

  /// <summary>Safety device cut out or isolated by the driver (保安装置開放).</summary>
  BIsolated,

  /// <summary>Fault or error condition reported by the safety device (故障).</summary>
  Failure,

  /// <summary>Notice to switch over safety system: ATS/ATC, line ruleset, depot/test mode (ATS/ATC切替).</summary>
  ModeSelect,

  /// <summary>Unclassified or sim-specific state.</summary>
  Other
}
