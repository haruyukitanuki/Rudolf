/** Sim clock and frame counter for an `OutputDataFrame`. */
export interface Time {
  /** `"HH:MM:SS"` when `dateKnown` is false; ISO datetime when true. */
  sim: string;
  /** True when the sim provides a real date (most train sims do not). */
  dateKnown: boolean;
  /** Seconds since scenario start; monotonic. */
  elapsed: number;
  /** Frame counter; increments each emit. */
  tick: number;
}

export const emptyTime = (): Time => ({
  sim: '00:00:00',
  dateKnown: false,
  elapsed: 0,
  tick: 0
});
