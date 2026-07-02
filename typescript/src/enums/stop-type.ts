/** Reason for stop at a station. */
export const StopType = {
  /** For standard revenue service, stop for passengers. */
  PassengerStop: 'PassengerStop',
  /** For operational reasons such as signal station turnback, parking, etc. */
  OperationStop: 'OperationStop',
  /** Pass through without stopping. */
  Passing: 'Passing'
} as const satisfies Record<string, string>;

export type StopType = (typeof StopType)[keyof typeof StopType];
