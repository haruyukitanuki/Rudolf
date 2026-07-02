import type { Direction } from '../enums/direction';
import type { PantographDirection } from '../enums/pantograph-direction';
import type { PantographType } from '../enums/pantograph-type';

/** Static composition for a single car (cabs, motors, pantograph layout). */
export interface CarStaticInfo {
  /** Matches `Car.carNo`. */
  carNo: number;
  /** Per-car model code (e.g. `"KuHaE233"`, `MoHa225-51xx`). For maximum interoperability, romanise all kana in TitleCase. */
  model: string;
  /** True when this car has a driver's cab. */
  hasDriverCab: boolean;
  /** True when this car has a conductor's cab. */
  hasConductorCab: boolean;
  /** True when this car is motorized (M/MM'). */
  hasMotor: boolean;
  /** True when this car carries one or more pantographs. */
  hasPantograph: boolean;
  /** Which way this car's driver cab faces; null when the car has no driver cab. */
  cabDirection: Direction | null;
  /** Style of pantograph; null when `hasPantograph` is false. */
  pantographType: PantographType | null;
  /** Which end(s) the pantograph(s) lean toward; null when `hasPantograph` is false or unknown. */
  pantographDirection: PantographDirection | null;
  /** Car length in meters. -1 if unknown. */
  length: number;
}

/** Vehicle identity plus static per-car composition (cabs, motors, pantographs). */
export interface VehicleInfo {
  /**
   * Human display name for the formation (e.g. `"225系0番台"`). Please ensure the correct kanji is used
   * for kei and bandai.
   */
  name: string;
  /**
   * Vehicle/formation model identifier (e.g. `"E233-1000"`). For maximum interoperability, it should
   * be in format of `series-subseries`; Avoid using Japanese characters.
   */
  model: string;
  /**
   * Operating company (e.g. `"EastJapanRailwayCompany"`, `"TokyuCorporation"`).
   * For maximum compatibility, refer to Japanese Wikipedia for the full operator name (not group) and TitleCase it.
   */
  operator: string;
  /**
   * One entry per car. List is displayed from left to right.
   * The first car in the list doesn't always represent lead.
   */
  cars: CarStaticInfo[];
  /** Car no. for the lead car. This should usually be the car number of either the first or last item in `cars`. */
  leadCar: number;
}
