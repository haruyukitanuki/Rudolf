/** Panel-lamp vocabulary overrides: typically maps opaque integer panel indices to named keys. */
export interface LampVocabulary {
  /** Panel-array index (string-keyed) → named lamp key. */
  bveIndexToKey?: Record<string, string>;
}

/**
 * Vocabulary overrides for this sim+vehicle. Lets a consumer translate sim-specific
 * indices (panel-lamp arrays, custom signal aspects, beacon codes) into
 * human-meaningful labels without hard-coding per-vehicle knowledge in the UI.
 */
export interface Vocabularies {
  /** Panel-lamp vocabulary overrides; null/undefined when no overrides apply. */
  lamps?: LampVocabulary;
  /** Per-route overrides for the default 0-7 signal-phase vocabulary. */
  signalPhase?: Record<string, string>;
  /** Sim-known beacon type code → human-readable meaning. */
  transponders?: Record<string, string>;
}
