import type { RudolfDocumentBase } from './envelope';
import type { Capabilities } from './profile/capabilities';
import type { ScenarioInfo } from './profile/scenario-info';
import type { SimInfo } from './profile/sim-info';
import type { VehicleInfo } from './profile/vehicle-info';
import type { Vocabularies } from './profile/vocabularies';

/**
 * Once-per-scenario descriptor of what the sim+vehicle can and cannot fill in subsequent
 * `OutputDataFrame` documents. Sent once at scenario load (and again on vehicle change),
 * cached by `scenarioId`. Lets consumers render "N/A" instead of misleading zeros
 * for fields the sim cannot source.
 */
export interface SimulatorProfile extends RudolfDocumentBase {
  kind: 'SimulatorProfile';
  /** Simulator + adapter identity (name + version). */
  sim: SimInfo;
  /** Scenario metadata (title, route, author, start time, diagram info). */
  scenario: ScenarioInfo;
  /** Vehicle identity plus static per-car composition (cabs, motors, pantographs). */
  vehicle: VehicleInfo;
  /** Capability map declaring which `OutputDataFrame` fields are reliably populated. */
  capabilities: Capabilities;
  /** Vocabulary overrides (lamps, signal phase, transponder codes) for this sim+vehicle. */
  vocabularies: Vocabularies;
}
