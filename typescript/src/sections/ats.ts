import type { AtsRichStateType } from '../enums/ats-rich-state-type';

/** Machine-readable ATS events (alternative to the free-form `Ats.state` string). Parallel arrays: index N across all fields describes the Nth active state. */
export interface AtsRichState {
  /** Stable enum-like codes (e.g. `'P_APPROACH'`, `'EB'`). */
  code: string[];
  /** Human-readable display labels (e.g. `'P接近'`). */
  name: string[];
  /** 0 = info, 1 = warning, 2 = critical. */
  severity: (0 | 1 | 2)[];
  /** Machine-readable event category for each state (parallel with `code`). */
  type: AtsRichStateType[];
}

/**
 * ATS/ATC state.
 * Covers the speed cap currently asserted by ATS plus an optional rich-state object
 * for richer per-family info (P established, EB engaged, etc.).
 */
export interface Ats {
  /** Free-form class identifier when the sim natively exposes one; null otherwise. */
  class: string | null;
  /** Current ATS speed limit in km/h. `-1` = free, `null` = blank display, any other number = the asserted cap. */
  speed: number | null;
  /** Free-form rich-state string (e.g. `'P接近'`, `'B動作'`, `'EB'`); null when not asserted. */
  state: string | null;
  /** Structured rich-state alternative to `state`; null when the family profile doesn't fill it. */
  richState: AtsRichState | null;
}

export const emptyAts = (): Ats => ({
  class: null,
  speed: null,
  state: null,
  richState: null
});
