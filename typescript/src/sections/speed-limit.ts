import type { SpeedLimitType } from '../enums/speed-limit-type';

/** The next speed-limit change ahead of the train. */
export interface SpeedLimitNext {
  /** Next speed limit in km/h. */
  limit: number;
  /** Next speed limit section in meters. */
  distance: number;
  /** Type of speed limit. */
  type: SpeedLimitType | null;
}

/** Currently enforced speed limit plus the upcoming change points. */
export interface SpeedLimit {
  /** Current speed limit in km/h. */
  current: number;
  /** Origin of the current limit (posted, signal-imposed, or restriction); null when unknown. */
  currentType: SpeedLimitType | null;
  /**
   * Upcoming speed-limit changes ahead, ordered nearest-first (ascending `distance`), so `next[0]`
   * is the closest change. Producers that only know the immediate next change emit a single-element
   * array; producers that know the full forward sequence emit them all (see
   * `capabilities['speedLimit.next']`). `null` when no upcoming change is known.
   */
  next: SpeedLimitNext[] | null;
}

export const emptySpeedLimit = (): SpeedLimit => ({
  current: 0,
  currentType: null,
  next: null
});
