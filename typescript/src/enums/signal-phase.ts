/**
 * Signal aspect index. Serialized as its underlying int.
 * Proceed-ordered (more permissive = higher number); `0` reserved for non-functional signals.
 *
 * - `0` = non-functional or disabled
 * - `1` = R (停止)
 * - `2` = YY (警戒)
 * - `3` = Y (注意)
 * - `4` = YG (減速)
 * - `5` = YGF (抑速)
 * - `6` = G (進行)
 * - `7` = GG (高速進行)
 * - `8+` = sim/vehicle-specific (see `SimulatorProfile.vocabularies.signalPhase`)
 */
export const SignalPhase = {
  /** Signal is non-functional or disabled. */
  Disabled: 0,
  /** Stop (停止). */
  R: 1,
  /** Restricted speed (警戒). */
  YY: 2,
  /** Caution (注意). */
  Y: 3,
  /** Reduced speed (減速). */
  YG: 4,
  /** Limited speed (抑速). */
  YGF: 5,
  /** Proceed (進行). */
  G: 6,
  /** High-speed Proceed (高速進行). */
  GG: 7
} as const satisfies Record<string, number>;

export type SignalPhase = (typeof SignalPhase)[keyof typeof SignalPhase];
