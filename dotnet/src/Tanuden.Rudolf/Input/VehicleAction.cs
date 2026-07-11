using System.Text.Json.Serialization;

namespace Tanuden.Rudolf.Input;

/// <summary>
///   Physical cab/train control actions referenced by <c>SetButtonCommand</c>.
///   Each value is a button-press or key-press style event (some are momentary, some are toggles).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VehicleAction
{
  /// <summary>Reset the EB/deadman alarm (EB復帰).</summary>
  EBReset,

  /// <summary>Engage the gradient-start / anti-rollback switch (勾配起動スイッチ).</summary>
  GradientStart,

  /// <summary>Safety-brake switch (保安ブレーキ).</summary>
  SafetyBrake,

  /// <summary>Snow-resistance brake switch (耐雪ブレーキ).</summary>
  SnowBrake,

  /// <summary>Sound the air horn (空気笛).</summary>
  HornAir,

  /// <summary>Sound the electric horn (電気笛).</summary>
  HornElectric,

  /// <summary>Press the cab buzzer (合図ブザー).</summary>
  Buzzer,

  /// <summary>Open the left-side passenger doors (左ドア開).</summary>
  DoorOpenLeft,

  /// <summary>Close the left-side passenger doors (左ドア閉).</summary>
  DoorCloseLeft,

  /// <summary>Open the right-side passenger doors (右ドア開).</summary>
  DoorOpenRight,

  /// <summary>Close the right-side passenger doors (右ドア閉).</summary>
  DoorCloseRight,

  /// <summary>Re-open/re-close switch after an interrupted closure (再開閉SW).</summary>
  DoorReopen,

  /// <summary>Door-switch key operation (ドアスイッチ鍵).</summary>
  DoorKey,

  /// <summary>3/4-door partial-open switch (3/4閉スイッチ).</summary>
  PartialDoor,

  /// <summary>Door cut-out switch (ドアカットSW).</summary>
  DoorCut,

  /// <summary>Boarding-prompt buzzer (乗降促進).</summary>
  BoardingPrompt,

  /// <summary>Trigger an in-car announcement / PA (車内放送).</summary>
  InCarBroadcast,

  /// <summary>Dim the headlight / low beam (前灯減光).</summary>
  HeadLightLow,

  /// <summary>Headlight switch (前照灯SW).</summary>
  HeadLight,

  /// <summary>Passenger-cabin light switch (客室灯SW).</summary>
  CabinLight,

  /// <summary>Crew-room light switch (乗務員室灯SW).</summary>
  CrewRoomLight,

  /// <summary>Instrument/meter light switch (計器灯SW).</summary>
  InstrumentLight
}
