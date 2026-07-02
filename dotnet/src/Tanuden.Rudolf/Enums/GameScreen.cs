using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Enums;

/// <summary>Which outer-game screen is currently visible.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameScreen
{
  /// <summary>Active gameplay (driving the train)</summary>
  MainGame,

  /// <summary>Game is paused mid-scenario</summary>
  Pause,

  /// <summary>Loading a scenario/vehicle/route</summary>
  Loading,

  /// <summary>In a menu (settings, scenario selector, etc.)</summary>
  Menu,

  /// <summary>Post-scenario result/scoring screen</summary>
  Result,

  /// <summary>Title screen</summary>
  Title,

  /// <summary>Sim process is not running; default value</summary>
  NotRunning,

  /// <summary>Some other screen the schema doesn't name explicitly</summary>
  Other
}
