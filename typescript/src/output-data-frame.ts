import type { RudolfDocumentBase } from './envelope';
import type { Ats } from './sections/ats';
import type { Cars } from './sections/cars';
import type { Controllers } from './sections/controllers';
import type { Diagram } from './sections/diagram';
import type { Doors } from './sections/doors';
import type { GameState } from './sections/game-state';
import type { Lamps } from './sections/lamps';
import type { Physics } from './sections/physics';
import type { Signals } from './sections/signals';
import type { SpeedLimit } from './sections/speed-limit';
import type { Stations } from './sections/stations';
import type { Switches } from './sections/switches';
import type { Time } from './sections/time';

/**
 * Per-frame snapshot of dynamic train and cab state emitted by a simulator (sim → consumer).
 * Sent at ~4 Hz/every sim tick. Pair with a `SimulatorProfile` sharing the same
 * `scenarioId` for capability flags and vocabulary overrides.
 */
export interface OutputDataFrame extends RudolfDocumentBase {
  kind: 'OutputDataFrame';
  /** Sim clock + frame counter. */
  time: Time;
  /** Train identity (number, bound-for, service class) for this run. */
  diagram: Diagram;
  /** Ordered station list for the diagram plus current/next indices. */
  stations: Stations;
  /** Train-aggregate physics (speed, distance, MR pressure, gradient). */
  physics: Physics;
  /** Driver-input state (notches, reverser, ATO/TASC/deadman). */
  controllers: Controllers;
  /** Door state: safe-to-proceed flag plus per-car open-side. */
  doors: Doors;
  /** Vocabulary-keyed panel-lamp values (0=off, 1=on, 2+=vehicle-specific). */
  lamps: Lamps;
  /** ATS pattern speed, class, and rich-state info. */
  ats: Ats;
  /** Upcoming signals with aspect, distance, and transponders. */
  signals: Signals;
  /** Current speed limit and the next change point. */
  speedLimit: SpeedLimit;
  /** Per-car dynamic state (BC pressure, amperage, occupancy). */
  cars: Cars;
  /** Cab switch state (horn, buzzer, headlights, wiper). */
  switches: Switches;
  /** Outer game-state context (screen, crew role, drive mode, one-man). */
  gameState: GameState;
  /** Sim/vendor-specific blocks keyed `"<namespace>:<concern>"`. */
  extensions?: Record<string, unknown>;
}
