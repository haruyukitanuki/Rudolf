/**
 * Discrete cab-input actions referenced by `SetButtonCommand`.
 * Each value is a button-press or key-press style event (some are momentary, some are toggles).
 */
export const InputAction = {
  /** Increment the combined notch by one step (toward power). */
  NotchUp: 'NotchUp',
  /** Decrement the combined notch by one step (toward brake). */
  NotchDown: 'NotchDown',
  /** Snap the combined notch to neutral. */
  NotchN: 'NotchN',
  /** Step the combined notch one position toward neutral (from either direction). */
  NotchTowardN: 'NotchTowardN',
  /** Snap the brake notch to emergency (非常). */
  NotchEB: 'NotchEB',
  /** Snap the brake notch to B1. */
  NotchB1: 'NotchB1',
  /** Reset the EB/deadman alarm (EB復帰). */
  EBReset: 'EBReset',
  /** Start the gradient (regen-braking adjustment) sequence. */
  GradientStart: 'GradientStart',
  /** Sound the air horn (空気笛). */
  HornAir: 'HornAir',
  /** Sound the electric horn (電気笛). */
  HornElectric: 'HornElectric',
  /** Press the cab buzzer (合図ブザー). */
  Buzzer: 'Buzzer',
  /** Open doors. */
  DoorOpen: 'DoorOpen',
  /** Close doors. */
  DoorClose: 'DoorClose',
  /** Re-open doors after a closure attempt was interrupted (再開閉). */
  DoorReopen: 'DoorReopen',
  /** Activate the door key (rare; conductor cab unlock). */
  DoorKey: 'DoorKey',
  /** Trigger the boarding-prompt chime/announcement. */
  BoardingPrompt: 'BoardingPrompt',
  /** Trigger an in-train broadcast/PA event. */
  Broadcast: 'Broadcast',
  /** Toggle the low-beam headlight (前照灯). */
  LightLow: 'LightLow',
  /** Switch the conductor's view to look backward along the train. */
  ConductorViewBack: 'ConductorViewBack',
  /** Cycle to the next camera/view. */
  ViewChange: 'ViewChange',
  /** Open the pause menu. */
  PauseMenu: 'PauseMenu',
  /** Open the diagram/schedule view. */
  ViewDiagram: 'ViewDiagram',
  /** Open the in-game UI. */
  ViewUserInterface: 'ViewUserInterface',
  /** Return to the home/default view. */
  ViewHome: 'ViewHome',
  /** Driver camera: look out the left window. */
  DriverViewLeft: 'DriverViewLeft',
  /** Driver camera: look out the right window. */
  DriverViewRight: 'DriverViewRight',
  /** Driver camera: face forward (center). */
  DriverViewCenter: 'DriverViewCenter'
} as const satisfies Record<string, string>;

export type InputAction = (typeof InputAction)[keyof typeof InputAction];
