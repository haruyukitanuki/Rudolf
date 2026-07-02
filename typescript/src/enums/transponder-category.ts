/**
 * Functional category of a track-side transponder/beacon.
 * Adapters MUST emit `null` rather than guess when they can't categorize.
 */
export const TransponderCategory = {
  /** Pattern source transponder. */
  Pattern: 'Pattern',
  /** Signal transponder. */
  Signal: 'Signal',
  /** TASC transponder. */
  TASC: 'TASC',
  /** Recognized but does not fit any specific category. */
  Other: 'Other'
} as const satisfies Record<string, string>;

export type TransponderCategory = (typeof TransponderCategory)[keyof typeof TransponderCategory];
