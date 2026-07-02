/**
 * Discriminator string identifying which RUDOLF document is on the wire.
 * Consumers narrow `RudolfDocumentBase` by checking `kind`.
 */
export type RudolfDocumentKind = 'SimulatorProfile' | 'OutputDataFrame' | 'InputCommand';

/** Shared envelope fields present on every RUDOLF document. */
export interface RudolfDocumentBase {
  /** RUDOLF schema version this document conforms to. */
  schemaVersion: string;
  /** Document discriminator. */
  kind: RudolfDocumentKind;
  /** Opaque identifier tying all documents of one play-session together. */
  scenarioId: string;
  /** ISO 8601 timestamp at the producer. */
  sentAt: string;
}
