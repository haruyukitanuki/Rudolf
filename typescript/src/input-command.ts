import type { RudolfDocumentBase } from './envelope';
import type { Command } from './input/command';

/**
 * Single driver-input event sent from a consumer back into the simulator (consumer → sim).
 * Carries one `Command` payload (a `SetXxxCommand` subtype) plus a
 * `sequenceNumber` for ordering and idempotency.
 */
export interface InputCommand extends RudolfDocumentBase {
  kind: 'InputCommand';
  /** Monotonic per consumer; for ordering/idempotency. */
  sequenceNumber: number;
  /** The command payload. Discriminated on its `kind` string at the wire level. */
  command: Command;
}
