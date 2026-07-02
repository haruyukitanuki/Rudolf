using Tanuden.Rudolf.Enums;

namespace Tanuden.Rudolf.Sections;

/// <summary>Outer-game context: which screen is showing, what role the player is, etc.</summary>
public class GameState
{
  /// <summary>Which screen is currently visible (main game, pause, menu, loading, ...).</summary>
  public GameScreen Screen = GameScreen.NotRunning;

  /// <summary>Player's current role; null when the sim doesn't model crew role.</summary>
  public CrewRole? CrewRole;

  /// <summary>Whether the run is being scored/graded; null when the sim has no drive-mode distinction.</summary>
  public DriveMode? DriveMode;

  /// <summary>One-man operation.</summary>
  public bool IsOneman;
}
