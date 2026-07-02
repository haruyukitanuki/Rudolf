/** Origin of a speed-limit value. */
export const SpeedLimitType = {
  /** Imposed by an upcoming signal aspect. */
  Signal: 'Signal',
  /** Posted safe limit for stretch of track. */
  SpeedLimit: 'SpeedLimit',
  /** Temporary speed restricted section. */
  Restriction: 'Restriction'
} as const satisfies Record<string, string>;

export type SpeedLimitType = (typeof SpeedLimitType)[keyof typeof SpeedLimitType];
