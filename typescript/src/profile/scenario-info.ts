/** Scenario metadata. */
export interface ScenarioInfo {
  /** Scenario title. */
  title: string;
  /** Route identity (e.g. file path stem or route-pack name). */
  route: string;
  /** Scenario author (if the simulator exposes it). null otherwise. */
  author: string | null;
  /** Bare `"HH:MM:SS"`. This does not include date. */
  scenarioStartTime: string;
  /** Train/diagram number when known at scenario load. Mirrors `Diagram.trainNumber`. */
  diagramNumber: string | null;
  /** Destination when known at scenario load. Mirrors `Diagram.boundFor`. */
  boundFor: string | null;
  /** Service type when known at scenario load. Mirrors `Diagram.serviceType`. */
  serviceType: string | null;
}
