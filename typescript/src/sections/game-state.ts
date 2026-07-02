import type { CrewRole } from '../enums/crew-role';
import type { DriveMode } from '../enums/drive-mode';
import type { GameScreen } from '../enums/game-screen';

/** Outer-game context: which screen is showing, what role the player is, etc. */
export interface GameState {
  /** Which screen is currently visible (main game, pause, menu, loading, ...). */
  screen: GameScreen;
  /** Player's current role; null when the sim doesn't model crew role. */
  crewRole: CrewRole | null;
  /** Whether the run is being scored/graded; null when the sim has no drive-mode distinction. */
  driveMode: DriveMode | null;
  /** One-man operation. */
  isOneman: boolean;
}

export const emptyGameState = (): GameState => ({
  screen: 'NotRunning',
  crewRole: null,
  driveMode: null,
  isOneman: false
});
