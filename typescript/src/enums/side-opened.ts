/**
 * Which side(s) of a car are currently open. Serialized as its underlying int.
 *
 * - `-1` = Left
 * - `0` = Closed
 * - `1` = Right
 * - `2` = Both
 *
 * If `null` is used, it indicates opened but which side is unknown.
 */
export const SideOpened = {
  /** Left doors opened. */
  Left: -1,
  /** Closed. */
  Closed: 0,
  /** Right doors opened. */
  Right: 1,
  /** Doors on both sides opened. */
  Both: 2
} as const satisfies Record<string, number>;

export type SideOpened = (typeof SideOpened)[keyof typeof SideOpened];
