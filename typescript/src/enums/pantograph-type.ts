/** Mechanical style of a pantograph collector. */
export const PantographType = {
  /** Single-arm. */
  SingleArm: 'SingleArm',
  /** Scissor/diamond. */
  Scissor: 'Scissor'
} as const satisfies Record<string, string>;

export type PantographType = (typeof PantographType)[keyof typeof PantographType];
