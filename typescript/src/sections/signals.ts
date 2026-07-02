import type { SignalType } from '../enums/signal-type';
import type { TransponderCategory } from '../enums/transponder-category';

/** A single beacon/transponder placed in the trackbed. */
export interface Transponder {
  /** Functional category (Pattern, Signal, TASC, Other); null when unclassified. */
  category: TransponderCategory | null;
  /** Sim-native beacon type code; null when the sim doesn't expose it. */
  code: number | null;
  /** km/h imposed by this transponder, if applicable. */
  speedLimit: number | null;
  /** Meters; negative = already passed. */
  distance: number;
}

/** A single upcoming signal. */
export interface Signal {
  /** Sim-native signal name when available; some sims synthesize from block distance (e.g. `"SecXXXm"`). */
  name: string | null;
  /** Functional class of the signal (block, distant, call-on, shunt, home, departure). */
  type: SignalType | null;
  /** Signal aspect index. See `SignalPhase` for the default vocabulary. */
  phase: number | null;
  /** Meters ahead. */
  distance: number;
  /** Beacons/transponders associated with this signal block. */
  transponders: Transponder[];
}

/** Upcoming signals visible ahead of the train, each with its current aspect and transponders. */
export interface Signals {
  /** Signals in distance order (nearest first). Empty when no signals are visible/known. */
  list: Signal[];
}

export const emptySignals = (): Signals => ({
  list: []
});
