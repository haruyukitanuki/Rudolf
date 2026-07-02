/** Door state for a single car. */
export interface CarDoorState {
  /** Matches `CarStaticInfo.carNo`. */
  carNo: number;
  /**
   * Which side(s) are open. See `SideOpened` for the int convention.
   * `null` if doors opened but side unknown.
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
