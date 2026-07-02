/**
 * Capability flags keyed by dotted path matching `OutputDataFrame` field paths.
 *
 * Values are usually booleans; some keys carry string-enum values (e.g. `physics.perCar`
 * is `'true' | 'broadcast' | 'unavailable'`, `ats.richState` is `'rich' | 'eb-only' | 'none'`).
 * See the spec for the canonical list.
 */
export type Capabilities = Record<string, boolean | string>;
