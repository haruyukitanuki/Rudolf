import type { StopType } from '../enums/stop-type';

/** A single station entry within `Stations.list`. */
export interface Station {
  /** Position of this station within `Stations.list`. Useful for round-tripping. */
  index: number;
  /**
   * Station display name only (typically Japanese, e.g. `"新宿"`) — no station codes or numbering
   * (e.g. `"品川"`, never `"KK01 品川"` / `"品川(JK20)"` / `"KK01"`). Emitted as literal UTF-8,
   * with no `\u` escape sequences.
   */
  name: string;
  /** Meters from scenario start; always present. */
  fromStartDistance: number;
  /** Absolute kilometer-post; null when the sim doesn't expose chainage. */
  absoluteDistance: number | null;
  /** Which side the doors open. See `DoorSide` for the int convention. */
  doorSide: number;
  /** Whether this is a passenger stop, operation-only stop, or pass. */
  stopType: StopType | null;
  /** ISO datetime; null for first station and passing-only stops. */
  arrival: string | null;
  /** ISO datetime; null for last station. */
  departure: string | null;
  /** Platform/stop position name; null when not specified. */
  stopPositionName: string | null;
  /** True when this station is a timing point (採時駅); null when the sim doesn't model it. */
  isTimeTaken: boolean | null;
  /** Candidate stop-position markers as car-counts for this station's current direction/platform (e.g. `[3, 4, 6]`); null when unknown. */
  stopPositions: number[] | null;
}

/** Ordered station list for the diagram plus pointers into it. */
export interface Stations {
  /** Stations in scheduled visit order. May be empty before the diagram is loaded. */
  list: Station[];
  /** Index into `list` for the station the train is currently at; null when between stations. */
  currentIndex: number | null;
  /** Index into `list` for the next station ahead; null at terminus. */
  nextIndex: number | null;
}

export const emptyStations = (): Stations => ({
  list: [],
  currentIndex: null,
  nextIndex: null
});
