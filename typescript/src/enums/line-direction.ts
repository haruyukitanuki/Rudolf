/** Direction relative to the line. */
export const LineDirection = {
  /** Upbound. */
  Left: 'Left',
  /** Downbound. */
  Right: 'Right'
} as const satisfies Record<string, string>;

export type LineDirection = (typeof LineDirection)[keyof typeof LineDirection];
