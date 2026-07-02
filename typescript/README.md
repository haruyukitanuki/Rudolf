# `@tanuden/rudolf`

TypeScript type definitions for the **Rudolf (Railway Unified Display Object Link Format)** wire schema.

## Install

```bash
npm install @tanuden/rudolf
```

## Usage

```ts
import type { OutputFrame, SimProfile, InputCommand } from '@tanuden/rudolf';
import { SCHEMA_VERSION, emptyPhysics, SignalPhase, DoorSide } from '@tanuden/rudolf';

const physics = emptyPhysics();
physics.speed = 78.4;

if (frame.signals.list[0]?.phase === SignalPhase.R) {
  // stop signal
}
```

This package contains **only** type definitions and small factory/constant helpers. No runtime validation, no transport, no business logic. It is environment-neutral (works in both browser and Node.js).

## Specification

See [Rudolf spec](https://github.com/haruyukitanuki/rudolf/blob/main/spec/rudolf-spec.md) for the full wire format.

## License

Apache 2.0.
