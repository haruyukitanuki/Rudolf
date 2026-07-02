/** Role the player is currently performing. */
export const CrewRole = {
  /** Driver. */
  Driver: 'Driver',
  /** Conductor. */
  Conductor: 'Conductor',
  /** Both Driver and Conductor. This usually indicates one-man operations. */
  Both: 'Both',
  /** Others. */
  Others: 'Others'
} as const satisfies Record<string, string>;

export type CrewRole = (typeof CrewRole)[keyof typeof CrewRole];
