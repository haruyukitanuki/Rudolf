using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Input;

/// <summary>
///   Camera/view/UI/sim-meta actions referenced by <c>SetButtonCommand</c>.
///   Each value is a button-press or key-press style event (some are momentary, some are toggles).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameAction
{
  /// <summary>Toggle the exterior/external view (外部視点切替).</summary>
  ExteriorView,

  /// <summary>Driver alternate viewpoint.</summary>
  DriverAlternateView,

  /// <summary>Conductor rear-confirmation view (後方確認).</summary>
  ConductorAlternateView,

  /// <summary>Look out of the left window.</summary>
  LeftWindowView,

  /// <summary>Look out of the right window.</summary>
  RightWindowView,

  /// <summary>Toggle the pause menu.</summary>
  TogglePauseMenu,

  /// <summary>Toggle the diagram/schedule display (スタフ表示).</summary>
  ToggleDiagramDisplay,

  /// <summary>Toggle the in-game UI (画面表示).</summary>
  ToggleGUI,

  /// <summary>Toggle the crew door.</summary>
  ToggleCrewDoor,

  /// <summary>Toggle the crew window.</summary>
  ToggleCrewWindow
}
