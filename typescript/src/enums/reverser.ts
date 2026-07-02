/**
 * Reverser position. Adapters MUST clamp out-of-range sim values into this set. All other values are not acceptable.
 */
export const Reverser = {
  /** Reverse (後進). */
  Reverse: -1,
  /** Neutral (中立). */
  Neutral: 0,
  /** Forward (前進). */
  Forward: 1
} as const satisfies Record<string, number>;

export type Reverser = (typeof Reverser)[keyof typeof Reverser];
