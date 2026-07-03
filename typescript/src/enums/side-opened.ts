/**
 * Which side(s) of a car are currently open. Serialized as its underlying int.
 *
 * - `-1` = Left
 * - `0` = Closed
 * - `1` = Right
 * - `2` = Both
 * - `3` = Open, side unknown (doors open but L/R can't be distinguished)
 *
 * `null` means no per-car door value is available (spec §3.1), not "open, side unknown".
 */
export const SideOpened = {
  /** Left doors opened. */
  Left: -1,
  /** Closed. */
  Closed: 0,
  /** Right doors opened. */
  Right: 1,
  /** Doors on both sides opened. */
  Both: 2,
  /** Doors open but which side can't be distinguished. */
  OpenSideUnknown: 3
} as const satisfies Record<string, number>;

export type SideOpened = (typeof SideOpened)[keyof typeof SideOpened];
