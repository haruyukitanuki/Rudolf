/** Which outer-game screen is currently visible. */
export const GameScreen = {
  /** Active gameplay (driving the train). */
  MainGame: 'MainGame',
  /** Game is paused mid-scenario. */
  Pause: 'Pause',
  /** Loading a scenario/vehicle/route. */
  Loading: 'Loading',
  /** In a menu (settings, scenario selector, etc.). */
  Menu: 'Menu',
  /** Post-scenario result/scoring screen. */
  Result: 'Result',
  /** Title screen. */
  Title: 'Title',
  /** Sim process is not running; default value. */
  NotRunning: 'NotRunning',
  /** Some other screen the schema doesn't name explicitly. */
  Other: 'Other'
} as const satisfies Record<string, string>;

export type GameScreen = (typeof GameScreen)[keyof typeof GameScreen];
