/** Train-aggregate physical state. Per-car values live in `Cars`. */
export interface Physics {
  /** km/h; train-level; always present. */
  speed: number;
  /** Meters traveled from scenario start; always present. */
  fromStartDistance: number;
  /** Absolute kilometer-post; null when sim doesn't expose chainage. */
  absoluteDistance: number | null;
  /** Per mille; null when the sim doesn't expose gradient. */
  gradient: number | null;
  /** MR Pressure in kPa; train-level; always present. */
  mrPressure: number;
}

export const emptyPhysics = (): Physics => ({
  speed: 0,
  fromStartDistance: 0,
  absoluteDistance: null,
  gradient: null,
  mrPressure: 0
});
