/** Door state for a single car. */
export interface CarDoorState {
  /** Matches `CarStaticInfo.carNo`. */
  carNo: number;
  /**
   * Which side(s) are open. See `SideOpened` for the int convention.
   * `3` (`SideOpened.OpenSideUnknown`) if doors open but side unknown;
   * `null` only if no value is available (spec §3.1).
   */
  sideOpened: number | null;
}

/** Door state: a load-bearing safe-to-proceed flag plus per-car open-side detail. */
export interface Doors {
  /** Pilot lamp. */
  allClosed: boolean;
  /** Per-car door state. Empty when the sim doesn't expose per-car detail. */
  perCar: CarDoorState[];
}

export const emptyDoors = (): Doors => ({
  allClosed: true,
  perCar: []
});
