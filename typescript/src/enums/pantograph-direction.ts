/** Which end of the car a pantograph leans toward (TIMS-relative); Both = both ends. */
export const PantographDirection = {
  /** Leans toward the left (TIMS-relative). */
  Left: 'Left',
  /** Leans toward the right (TIMS-relative). */
  Right: 'Right',
  /** Leans toward both ends. */
  Both: 'Both'
} as const satisfies Record<string, string>;

export type PantographDirection = (typeof PantographDirection)[keyof typeof PantographDirection];
