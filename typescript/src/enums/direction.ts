/** Direction relative on TIMS screen. */
export const Direction = {
  /** Facing left on TIMS. */
  Left: 'Left',
  /** Facing right on TIMS. */
  Right: 'Right'
} as const satisfies Record<string, string>;

export type Direction = (typeof Direction)[keyof typeof Direction];
