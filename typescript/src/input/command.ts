import type {
  SetAtoNotchCommand,
  SetBrakeNotchCommand,
  SetBrakeSAPCommand,
  SetButtonCommand,
  SetDeadmanCommand,
  SetNotchCommand,
  SetPowerNotchCommand,
  SetReverserCommand,
  SetWiperCommand
} from './commands';

/**
 * Discriminated-union command type. The wire-level `kind` string discriminator
 * picks the concrete subtype (`SetNotchCommand`, `SetButtonCommand`, ...).
 */
export type Command =
  | SetNotchCommand
  | SetPowerNotchCommand
  | SetBrakeNotchCommand
  | SetBrakeSAPCommand
  | SetReverserCommand
  | SetButtonCommand
  | SetWiperCommand
  | SetAtoNotchCommand
  | SetDeadmanCommand;
