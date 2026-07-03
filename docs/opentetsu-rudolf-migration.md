# Migration from OpenTetsu

Differences between schemas:
- `train.speed` → `physics.speed`
- `train.distance` → `physics.fromStartDistance` (new sibling: `physics.absoluteDistance` for kilometer-post)
- `train.gradient` → `physics.gradient`
- `train.cars[]` → `cars.list[]`
- `train.lamps.{atsReady,...}` → `lamps.values.{atsReady,...}` (now int, 0/1/2+)
- `signalStates[]` → `signals.list[]`
- `signalStates[].phase` (string) → `signals.list[].phase` (int)
- `atsState.{state,speed,stopPattern}` → `ats.{state,speed,class}` (renamed)
- `diagram.{direction,boundFor,...}` → `diagram.{direction,boundFor,...}` (mostly preserved; runNumber/direction now NULL where sim doesn't know)
- `controllerState.{notch,bNotch,pNotch,reverser}` → `controllers.{brakeNotch,powerNotch,reverser}` (combined-notch dropped; reverser stays int with same -1/0/1 convention but explicitly typed)
- Static per-car data (`cars[].model`, `cars[].properties.{hasMotor,...}`) → `SimulatorProfile.vehicle.cars[]`
- `Transponder.speedlimit` (lowercase typo) → `signals.list[].transponders[].speedLimit` (correct camelCase)
- `Station.doorDirection: 'LeftSide' | 'RightSide'` (string enum) → `stations.list[].doorSide: int` (-1=Left, 0=None/Unknown, 1=Right, 2=Both)
- `CarState.isDoorClosed: bool` → `doors.perCar[].sideOpened: int | null` (same -1/0/1/2 convention as `doorSide`, plus `3` = open-side-unknown; `0` means closed, `3` means open-side-unknown, `null` means no value available). Per-car door state moved entirely out of `cars[]`.
- `nextStation: NextStation` (full duplicated record) → `stations.nextIndex` (consumer looks up via `stations.list[nextIndex]`). `currentStation` similarly removed in favor of `stations.currentIndex`.
- `Station.distance` → `stations.list[].fromStartDistance` (renamed for clarity; new sibling `stations.list[].absoluteDistance` for kilometer-post). `NextStation.distanceFromTrain` becomes `list[nextIndex].fromStartDistance - physics.fromStartDistance` at the consumer.
- `diagram.{remainingDistance, direction='Both'}` → removed (`remainingDistance` derivable; `direction` typed as `LineDirection`: `'Left' | 'Right' | null` where `Left` = Upbound and `Right` = Downbound).
- `atsState.speed: number` (with TC sending `"F"` requiring magic 300) → `ats.speed: number | null` with `-1 = free`, `null = blank display`. All-numeric on the wire.
- `Transponder.type: string` (free-form) → typed `{ category: enum, code: int|null, speedLimit: number|null, distance: number }`. Category vocabulary: `Pattern | Signal | TASC | Other | null`: collapsed from the longer OpenTetsu/early-draft set so adapters only commit to coarse categories they can reliably emit.
- `speedLimit.{currentType, next[].type}` vocabulary expanded with `'Restriction'` (curve / weather / work-zone / station-approach / slow-order).
- `speedLimit.next` is now a **list** (`SpeedLimitNext[] | null`) of upcoming limit changes, ordered nearest-first (`next[0]` = closest), rather than a single object. Producers emit one element (immediate next only) or the full forward sequence per `capabilities['speedLimit.next']` (`'single'` / `'full'`); `null` when none known.
- Signal `phase` int vocabulary transposed: `0 = disabled/broken/unknown` (NEW sentinel distinct from R), `1..7` are proceed-ordered with `YGF` correctly slotted between `YG` and `G`. BVE adapter MUST `+1` to `Section.CurrentSignalIndex` when emitting.
- `Switches.{Buzzer_M, Buzzer_C, Buzzer_Auto, HighBeam}` → `switches.{buzzerDriver, buzzerConductor, highBeam}` (auto removed; M/C renamed to initiator-clarity names). `Switches.headlights` is now `bool` (was unmapped); use `switches.highBeam: bool` for low/high beam distinction.
- New: `controllers.tasc: { active, notch?, inching } | null` for Train Automatic Stop Control observability (mirrors `controllers.ato` with extra `inching` flag for final low-speed alignment phase).
- New (sourced from hakata route/rolling-stock ROM): `stations.list[].isTimeTaken: bool | null` (timing point) and `stations.list[].stopPositions: number[] | null` (candidate stop-marker car-counts, direction/platform-resolved), bumping `stations.v` to `1.1`. Also adds `SimulatorProfile.vehicle.cars[].pantographDirection: 'Left' | 'Right' | 'Both' | null` (dedicated `PantographDirection` enum, alongside the existing `pantographType`).
