/**
 * Panel-lamp state, vocabulary-keyed.
 *
 * Values: `0` = off, `1` = on, `2+` = vehicle-specific alternative (blinking, dim, multicolor).
 * Basic TIMS that only knows 0/1 SHOULD treat any nonzero as truthy.
 *
 * Default keys include `doorClose`, `atsReady`, `atsBrakeApply`, `atsOpen`,
 * `regenerative`, `ebTimer`, `emergencyBrake`, `overload`, `pilot`, `ato`.
 * Sim/vehicle-specific keys are allowed freely.
 */
export interface Lamps {
  /** Lamp key → integer state. See class-level docs for the value convention. */
  values: Record<string, number>;
}

export const emptyLamps = (): Lamps => ({
  values: {}
});
