/** Functional classification of a signal head. */
export const SignalType = {
  /** Block signal (閉塞信号機). */
  Block: 'Block',
  /** Distant signal (遠方信号機). */
  Distant: 'Distant',
  /** Call-on signal (誘導信号機). */
  CallOn: 'CallOn',
  /** Shunt signal (入換信号機). */
  Shunt: 'Shunt',
  /** Home signal (場内信号機). */
  Home: 'Home',
  /** Departure signal (出発信号機). */
  Departure: 'Departure'
} as const satisfies Record<string, string>;

export type SignalType = (typeof SignalType)[keyof typeof SignalType];
