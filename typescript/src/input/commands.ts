import type { EBDeadmanMethod } from '../enums/eb-deadman-method';
import type { Reverser } from '../enums/reverser';
import type { Wiper } from '../enums/wiper';
import type { InputAction } from './input-action';

/** Set the combined power/brake notch (single-handle vehicles). */
export interface SetNotchCommand {
  kind: 'SetNotch';
  /** Signed combined notch: -8=EB, -7=B6 ... -2=B1, -1=抑速, 0=N, 1=P1 ... 5=P5. */
  value: number;
}

/** Set the power notch (two-handle vehicles). */
export interface SetPowerNotchCommand {
  kind: 'SetPowerNotch';
  /** 0 = neutral; positive integers are P1, P2, ... up to the vehicle's max. */
  value: number;
}

/** Set the brake notch (two-handle vehicles). */
export interface SetBrakeNotchCommand {
  kind: 'SetBrakeNotch';
  /** 0 = release; positive integers are B1, B2, ... up to EB. */
  value: number;
}

/** Set the Service Application Pressure on a self-lapping brake handle (SAP/自弁). */
export interface SetBrakeSAPCommand {
  kind: 'SetBrakeSAP';
  /** kPa; 0-400 = service, 410 = emergency. */
  kPa: number;
}

/** Set the reverser position. */
export interface SetReverserCommand {
  kind: 'SetReverser';
  /** Target reverser position. */
  value: Reverser;
}

/** Press, release, or toggle a discrete cab button. */
export interface SetButtonCommand {
  kind: 'SetButton';
  /** Which button. */
  action: InputAction;
  /** True = pressed/on; false = released/off. */
  state: boolean;
}

/** Set the wiper mode. */
export interface SetWiperCommand {
  kind: 'SetWiper';
  /** Target wiper mode. */
  state: Wiper;
}

/** Set the notch requested by an external ATO controller. */
export interface SetAtoNotchCommand {
  kind: 'SetAtoNotch';
  /** Signed combined notch in the same encoding as `SetNotchCommand.value`. */
  value: number;
}

/** Hold or release a deadman/EB channel. */
export interface SetDeadmanCommand {
  kind: 'SetDeadman';
  /** Which channel (hand/foot). */
  method: EBDeadmanMethod;
  /** True while the channel is held; false when released. */
  holding: boolean;
}
