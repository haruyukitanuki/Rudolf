/**
 * Which side the doors are expected to open at a station. Serialized as its underlying int.
 *
 * - `-1` = Left
 * - `0` = None/Unknown
 * - `1` = Right
 * - `2` = Both
 */
export const DoorSide = {
  /** Doors open on the left side. */
  Left: -1,
  /** No door opening expected, or the side is unknown. */
  None: 0,
  /** Doors open on the right side. */
  Right: 1,
  /** Doors open on both sides. */
  Both: 2
} as const satisfies Record<string, number>;

export type DoorSide = (typeof DoorSide)[keyof typeof DoorSide];
