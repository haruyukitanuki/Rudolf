/** Windshield wiper mode. */
export const Wiper = {
  /** Wipers off. */
  Off: 'Off',
  /** Intermittent wipe (間欠). */
  Intermittent: 'Intermittent',
  /** Continuous low-speed wipe. */
  Low: 'Low',
  /** Continuous high-speed wipe. */
  High: 'High'
} as const satisfies Record<string, string>;

export type Wiper = (typeof Wiper)[keyof typeof Wiper];
