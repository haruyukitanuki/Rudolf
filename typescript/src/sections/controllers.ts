import type { EBDeadmanMethod } from '../enums/eb-deadman-method';
import type { Reverser } from '../enums/reverser';

/** ATO state. */
export interface AtoState {
  /** True when ATO is engaged and currently driving. */
  active: boolean;
  /** The notch ATO is currently commanding; undefined when ATO is not asserting a value. */
  notch?: number;
}

/** Train Automatic Stop Controller state. */
export interface TascState {
  /** True when TASC is engaged. */
  active: boolean;
  /** The brake notch TASC is currently commanding; undefined when not asserting. */
  notch?: number;
  /** True during the final low-speed alignment phase against the platform stop marker. */
  inching: boolean;
}

/** Driver-input state: handles, reverser, and automatic driving systems. */
export interface Controllers {
  /** Current power notch. */
  powerNotch: number;
  /** Current brake notch. */
  brakeNotch: number;
  /** Reverser position. See `Reverser`. */
  reverser: Reverser;
  /** Auto Train Operation state; null when the vehicle has no ATO. */
  ato: AtoState | null;
  /** Train Automatic Stop Controller state; null when the vehicle has no TASC. */
  tasc: TascState | null;
  /** Which deadman/EB channel is currently engaged; null when nothing is engaged or the sim doesn't model deadman. */
  deadman: EBDeadmanMethod | null;
}

export const emptyControllers = (): Controllers => ({
  powerNotch: 0,
  brakeNotch: 0,
  reverser: 0,
  ato: null,
  tasc: null,
  deadman: null
});
