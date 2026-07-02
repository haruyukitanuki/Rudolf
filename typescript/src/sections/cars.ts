/** Dynamic state for a single car. */
export interface Car {
  /** Matches `CarStaticInfo.carNo`. */
  carNo: number;
  /** kPa; null when sim doesn't expose per-car BC pressure. */
  bcPressure: number | null;
  /** Amperes; null when sim doesn't expose per-car motor current. */
  amperage: number | null;
  /** Percentage filled (May exceed 100%). */
  occupancyRate: number | null;
}

/** Per-car dynamic state. Static composition lives in `VehicleInfo.cars`. */
export interface Cars {
  /**
   * One entry per car in left-to-right display order.
   * The first car in the list doesn't always represent lead.
   * Empty when the sim doesn't expose per-car detail.
   */
  list: Car[];
}

export const emptyCars = (): Cars => ({
  list: []
});
