import type { LineDirection } from '../enums/line-direction';

/**
 * Train-identity context for this run (train number, bound-for, service type, line direction).
 * All fields nullable: heuristic-derived fields (e.g. parsing the scenario title) MUST emit
 * null rather than guessing when the sim does not natively expose the value.
 */
export interface Diagram {
  /** Usually the same as Diagram Number. */
  trainNumber: string | null;
  /** The last station on the Diagram. */
  boundFor: string | null;
  /** Service Type. */
  serviceType: string | null;
  /** Operation direct relative to the line. */
  direction: LineDirection | null;
  /** Usually derived from Diagram Number using a formula dependent on the operator. */
  runNumber: string | null;
}

export const emptyDiagram = (): Diagram => ({
  trainNumber: null,
  boundFor: null,
  serviceType: null,
  direction: null,
  runNumber: null
});
