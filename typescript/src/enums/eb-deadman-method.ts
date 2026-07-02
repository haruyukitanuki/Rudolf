/** Which deadman/EB channel a `SetDeadman` command targets. */
export const EBDeadmanMethod = {
  /** Hand (e.g. T-shaped mascons). */
  Hand: 'Hand',
  /** Foot (e.g. older two-handed mascons). */
  Foot: 'Foot',
  /** EB (usually on modern EMUs with single handle mascon). */
  EB: 'EB'
} as const satisfies Record<string, string>;

export type EBDeadmanMethod = (typeof EBDeadmanMethod)[keyof typeof EBDeadmanMethod];
