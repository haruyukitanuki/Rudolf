/** Machine-readable category of an ATS event in `AtsRichState`. */
export const AtsRichStateType = {
  /** Flat continuous ceiling speed check (速度照査); no falling pattern active. Default cruising state when a fixed speed limit is being enforced. */
  SpeedCheck: 'SpeedCheck',
  /** Falling pattern enforced on a restrictive or stop signal: block, home, or departure (信号パターン). */
  SignalP: 'SignalP',
  /** Falling pattern enforced on a curve or turnout speed restriction (C信号). */
  CurveP: 'CurveP',
  /** Falling pattern protecting a track end or overrun-sensitive siding entry (終端パターン). */
  TerminalP: 'TerminalP',
  /** Warning that a falling pattern is being approached (P接近). */
  PApproach: 'PApproach',
  /** Chime sounding, awaiting driver confirmation before EB applies; common for ATS-S (確認扱い). */
  AckPending: 'AckPending',
  /** Service brake applied by the system (常用ブレーキ動作). */
  BApplication: 'BApplication',
  /** Emergency brake applied by the system (非常ブレーキ動作). */
  EbApplication: 'EbApplication',
  /** Falling pattern preventing station overruns and accidental passes (停車パターン・停通防止). */
  StopP: 'StopP',
  /** Traction power cut off by the system (ノッチカット). */
  NotchCut: 'NotchCut',
  /** Safety device cut out or isolated by the driver (保安装置開放). */
  BIsolated: 'BIsolated',
  /** Fault or error condition reported by the safety device (故障). */
  Failure: 'Failure',
  /** Notice to switch over safety system: ATS/ATC, line ruleset, depot/test mode (ATS/ATC切替). */
  ModeSelect: 'ModeSelect',
  /** Unclassified or sim-specific state. */
  Other: 'Other'
} as const satisfies Record<string, string>;

export type AtsRichStateType = (typeof AtsRichStateType)[keyof typeof AtsRichStateType];
