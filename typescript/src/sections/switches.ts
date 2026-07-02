import type { Wiper } from '../enums/wiper';

/** Cab-switch state: horn, buzzer, headlights, wiper. */
export interface Switches {
  /** True while the air horn is sounding. */
  hornAir: boolean;
  /** True while the electric horn is sounding. */
  hornElectric: boolean;
  /** Driver-initiated buzzer (sends to conductor). */
  buzzerDriver: boolean;
  /** Conductor-initiated buzzer (sends to driver). */
  buzzerConductor: boolean;
  /** True when headlights are on. Use `highBeam` to distinguish low/high beam. */
  headlights: boolean;
  /** True when the headlights are on high beam. Only meaningful when `headlights` is true. */
  highBeam: boolean;
  /** Wiper mode; null when the vehicle/sim has no wiper. */
  wiper: Wiper | null;
}

export const emptySwitches = (): Switches => ({
  hornAir: false,
  hornElectric: false,
  buzzerDriver: false,
  buzzerConductor: false,
  headlights: false,
  highBeam: false,
  wiper: null
});
