/** Type of driving run. */
export const DriveMode = {
  /** Scored/graded run (e.g. scenario mode). */
  Scored: 'Scored',
  /** For non-scored runs (e.g. free drive). */
  Unscored: 'Unscored',
  /** Some other mode the schema doesn't name explicitly. */
  Other: 'Other'
} as const satisfies Record<string, string>;

export type DriveMode = (typeof DriveMode)[keyof typeof DriveMode];
