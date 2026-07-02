using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Input;

/// <summary>
///   Discrete cab-input actions referenced by <c>SetButtonCommand</c>.
///   Each value is a button-press or key-press style event (some are momentary, some are toggles).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InputAction
{
  /// <summary>Increment the combined notch by one step (toward power).</summary>
  NotchUp,

  /// <summary>Decrement the combined notch by one step (toward brake).</summary>
  NotchDown,

  /// <summary>Snap the combined notch to neutral.</summary>
  NotchN,

  /// <summary>Step the combined notch one position toward neutral (from either direction).</summary>
  NotchTowardN,

  /// <summary>Snap the brake notch to emergency (非常).</summary>
  NotchEB,

  /// <summary>Snap the brake notch to B1.</summary>
  NotchB1,

  /// <summary>Reset the EB/deadman alarm (EB復帰).</summary>
  EBReset,

  /// <summary>Start the gradient (regen-braking adjustment) sequence.</summary>
  GradientStart,

  /// <summary>Sound the air horn (空気笛).</summary>
  HornAir,

  /// <summary>Sound the electric horn (電気笛).</summary>
  HornElectric,

  /// <summary>Press the cab buzzer (合図ブザー).</summary>
  Buzzer,

  /// <summary>Open doors.</summary>
  DoorOpen,

  /// <summary>Close doors.</summary>
  DoorClose,

  /// <summary>Re-open doors after a closure attempt was interrupted (再開閉).</summary>
  DoorReopen,

  /// <summary>Activate the door key (rare; conductor cab unlock).</summary>
  DoorKey,

  /// <summary>Trigger the boarding-prompt chime/announcement.</summary>
  BoardingPrompt,

  /// <summary>Trigger an in-train broadcast/PA event.</summary>
  Broadcast,

  /// <summary>Toggle the low-beam headlight (前照灯).</summary>
  LightLow,

  /// <summary>Switch the conductor's view to look backward along the train.</summary>
  ConductorViewBack,

  /// <summary>Cycle to the next camera/view.</summary>
  ViewChange,

  /// <summary>Open the pause menu.</summary>
  PauseMenu,

  /// <summary>Open the diagram/schedule view.</summary>
  ViewDiagram,

  /// <summary>Open the in-game UI.</summary>
  ViewUserInterface,

  /// <summary>Return to the home/default view.</summary>
  ViewHome,

  /// <summary>Driver camera: look out the left window.</summary>
  DriverViewLeft,

  /// <summary>Driver camera: look out the right window.</summary>
  DriverViewRight,

  /// <summary>Driver camera: face forward (center).</summary>
  DriverViewCenter
}
