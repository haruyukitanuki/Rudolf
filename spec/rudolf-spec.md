# Specifications for Rudolf

**English** | [日本語](./rudolf-spec.ja.md)

## 1. Nomenclature

| Term      | Meaning                                                                                                       |
| --------- | ------------------------------------------------------------------------------------------------------------- |
| Sim       | A train simulator (BVE, TRAIN CREW, etc.) that emits Rudolf documents.                                        |
| Adapter   | Code that reads a sim's native API and translates to Rudolf on the wire.                                      |
| Consumer  | Anything that reads Rudolf documents: HMI display, drive recorder, web dashboard, physical device controller. |
| Producer  | Anything that emits Rudolf documents: typically the adapter inside or alongside a sim.                        |
| Section   | A top-level key of an `OutputDataFrame` (e.g., `physics`, `signals`).                                         |
| Extension | A namespaced sim- or vendor-specific block under `extensions:` (e.g., `bve:beaconRing`).                      |
| Scenario  | One running play-session of a sim, from scenario load to scenario end.                                        |
| HMI       | Human Machine Interface, i.e. 列車情報管理装置 (TIMS/INTEROS).                                                |

## 2. Types of Documents

Rudolf defines three document types. All are JSON, UTF-8 encoded, camelCase.

| Document           | Direction      | Cadence                                      | Purpose                                        |
| ------------------ | -------------- | -------------------------------------------- | ---------------------------------------------- |
| `SimulatorProfile` | sim → consumer | Once per scenario load (and on vehicle swap) | Static metadata, capabilities, vocabulary maps |
| `OutputDataFrame`  | sim → consumer | Per-frame (typically 4 Hz/250 ms)            | Live train + game state snapshot               |
| `InputCommand`     | consumer → sim | Per input event                              | Device commands (notch, button, etc.)          |

Every document carries:

- `schemaVersion: string`: Rudolf spec version. Current version: `"1.0"`.
- `kind: 'SimulatorProfile' | 'OutputDataFrame' | 'InputCommand'`: discriminator.
- `scenarioId: string`: opaque identifier tying all documents of one play-session together. The same `scenarioId` appears on the SimulatorProfile, all OutputDataFrames in that scenario, and all InputCommands targeting it.
- `sentAt: string`: ISO 8601 timestamp at producer.

## 3. Architecture

### 3.1 Envelope conventions

#### Naming conventions

camelCase on the wire. C# producers convert from PascalCase via `CamelCasePropertyNamesContractResolver`. TypeScript/JavaScript consumers read camelCase directly.

#### String encoding

All string values are emitted as literal UTF-8 — **no `\uXXXX` escape sequences**. Japanese text (station names, route names, vehicle names) MUST appear as-is on the wire (e.g. `"立会川"`, not `"\u7ACB\u4F1A\u5DDD"`). Producers configure their serializer's encoder accordingly (e.g. .NET `JavaScriptEncoder.UnsafeRelaxedJsonEscaping`); the documents are never embedded raw in HTML, so HTML-escaping the output is unnecessary. Consumers MUST accept both forms regardless (`\u`-escaped JSON decodes to the same string).

#### Units

- speed: **km/h**
- pressure: **kPa**
- distance/location: **meters**
- gradient: **‰** (per mille)
- current: **A** (amperes)
- time: ISO 8601 strings, with `Kind=Unspecified` permitted for sims that lack real dates

#### Nullables

A field set to `null` means that "the sim really doesn't have this value right now."

A field that's absent from the JSON MEANS "the sim doesn't support this field at all" (also signaled in SimulatorProfile.capabilities). Producers SHOULD prefer `null` over absent for fields they generally produce but currently lack.

#### Versioning

All documents carry a single `schemaVersion` at the envelope level. A breaking change to any section bumps `schemaVersion`. Consumers MUST tolerate unknown fields added in future minor versions (read what they know, ignore what they don't).

### 3.2 Document structure

```
SimulatorProfile = { schemaVersion, kind, scenarioId, sentAt, sim, scenario, vehicle, capabilities, vocabularies }

OutputDataFrame = { schemaVersion, kind, scenarioId, sentAt,
                time, diagram, stations, physics, controllers, doors,
                lamps, ats, signals, speedLimit, cars, switches, gameState,
                extensions? }

InputCommand = { schemaVersion, kind, scenarioId, sentAt, sequenceNumber, command }
```

### 3.3 Extensibility

- **Extension blocks:** `extensions.<sim>:<concern>` namespacing. Examples: `bve:beaconRing`, `bve:atsPanelArray`. Third parties can author their own blocks freely.
- **Vocabularies:** Default vocabularies (signal phases, signal-phase speeds, lamp keys, transponder type meanings) are published in this spec. SimulatorProfile.vocabularies may override per-scenario.

## 4. SimulatorProfile

Sent once on scenario load. Re-sent on vehicle change. Cacheable by `scenarioId`.

```json
{
  "schemaVersion": "1.0",
  "kind": "SimulatorProfile",
  "scenarioId": "51a35aec-d930-455f-a8fa-58f686f87254",
  "sentAt": "2026-07-02T20:18:18.3444612+00:00",
  "sim": {
    "name": "TRAIN CREW",
    "version": "",
    "adapterName": "Tanuden.Rudolf.Adapters.TrainCrew",
    "adapterVersion": "0.1.0"
  },
  "scenario": {
    "title": "777",
    "route": "",
    "author": null,
    "scenarioStartTime": "00:00:00",
    "diagramNumber": "777",
    "boundFor": "館浜",
    "serviceType": "普通"
  },
  "vehicle": {
    "name": "4300",
    "model": "",
    "operator": "",
    "cars": [
      {
        "carNo": 1,
        "model": "4300",
        "hasDriverCab": true,
        "hasConductorCab": true,
        "hasMotor": true,
        "hasPantograph": false,
        "cabDirection": "Right",
        "pantographType": null,
        "pantographDirection": null,
        "length": -1
      },
      {
        "carNo": 2,
        "model": "4300",
        "hasDriverCab": false,
        "hasConductorCab": false,
        "hasMotor": false,
        "hasPantograph": true,
        "cabDirection": null,
        "pantographType": null,
        "pantographDirection": null,
        "length": -1
      },
      {
        "carNo": 3,
        "model": "4300",
        "hasDriverCab": false,
        "hasConductorCab": false,
        "hasMotor": false,
        "hasPantograph": false,
        "cabDirection": null,
        "pantographType": null,
        "pantographDirection": null,
        "length": -1
      },
      {
        "carNo": 4,
        "model": "4300",
        "hasDriverCab": true,
        "hasConductorCab": true,
        "hasMotor": true,
        "hasPantograph": false,
        "cabDirection": "Left",
        "pantographType": null,
        "pantographDirection": null,
        "length": -1
      }
    ],
    "leadCar": 4,
    "capabilities": {
      "masconType": "OneHandle",
      "masconBrakeType": "Notched",
      "powerNotches": 5,
      "brakeNotches": 8,
      "ebNotch": -8,
      "holdingBrakeNotches": 0,
      "cpStartPressure": 750,
      "cpStopPressure": 880
    }
  },
  "capabilities": {
    "physics.gradient": true,
    "physics.perCar": "true",
    "ats.richState": "rich",
    "speedLimit.next": "single",
    "input.command.SetNotch": true,
    "input.command.SetPowerNotch": true,
    "input.command.SetBrakeNotch": true,
    "input.command.SetBrakeSAP": true,
    "input.command.SetReverser": true,
    "input.command.SetButton": true,
    "input.command.SetWiper": true,
    "input.command.SetAtoNotch": true,
    "input.command.SetDeadman": true
  },
  "vocabularies": {
    "lamps": null,
    "signalPhase": null,
    "transponders": null,
    "signalPhaseSpeed": null
  }
}
```

### 4.1 `vehicle.capabilities`

Static control-hardware description for the vehicle — distinct from the top-level `capabilities` map (which declares which live `OutputDataFrame` fields the adapter populates). Every field is nullable; `null` means the sim has no value for it right now.

- `masconType` — master-controller handle layout: `'OneHandle' | 'TwoHandle' | null` (MasconType).
- `masconBrakeType` — brake-handle behaviour: `'Notched' | 'LapCapable' | 'Continuous' | null` (MasconBrakeType). `LapCapable` is controls with lap (so it automatically implies continuous); `Continuous` is a non-notched handle with no lap position (e.g. direct/straight-air controls).
- `powerNotches` — number of power notches (e.g. P1..P5 = 5); `null` when unknown.
- `brakeNotches` — number of service brake notches (e.g. B1..B7 = 7); `null` when unknown.
- `ebNotch` — signed notch value representing EB in the SetNotch encoding (e.g. `-8`); `null` when unknown.
- `holdingBrakeNotches` — number of holding-brake (抑速) notches; `0` when the vehicle has none, `null` when unknown.
- `cpStartPressure` / `cpStopPressure` — air-compressor cut-in / cut-out pressures, in kPa; `null` when unknown.

### 4.2 `vehicle.name`, `vehicle.model` & `vehicle.operator`

- `name` — human display name for the model (e.g. `"225系0番台"`). Ensure the correct kanji is used for kei (系) and bandai (番台). When the formation mixes more than one model, delimit them with a `+` (e.g. `"E231系1000番台+E233系3000番台"`).
- `model` — vehicle model identifier (e.g. `"225-0"`). For maximum interoperability it should be in `series-subseries` format; romanise all kana in TitleCase. When the formation mixes more than one model, delimit them with a `+` (e.g. `"E231-1000+E233-3000"`).
- `operator` — operating company (e.g. `"EastJapanRailwayCompany"`, `"TokyuCorporation"`). For maximum compatibility, refer to Japanese Wikipedia for the full operator name (not group) and TitleCase it.

## 5. OutputDataFrame

Sent per-frame (~4 Hz typical, sim may emit faster or slower). Every core section key is structurally present (even when empty); fields within sections may be null.

```jsonc
{
  "schemaVersion": "1.0",
  "kind": "OutputDataFrame",
  "scenarioId": "...",
  "sentAt": "2026-06-25T14:23:17.250Z",

  "time": {
    /* ... */
  },
  "diagram": {
    /* ... */
  },
  "stations": {
    /* ... */
  },
  "physics": {
    /* ... */
  },
  "controllers": {
    /* ... */
  },
  "doors": {
    /* ... */
  },
  "lamps": {
    /* ... */
  },
  "ats": {
    /* ... */
  },
  "signals": {
    /* ... */
  },
  "speedLimit": {
    /* ... */
  },
  "cars": {
    /* ... */
  },
  "switches": {
    /* ... */
  },
  "gameState": {
    /* ... */
  },

  "extensions": {
    // Optionals
    "bve:beaconRing": {
      /* ... */
    },
    "bve:atsPanelArray": {
      /* ... */
    },
  },
}
```

### 5.1 `time`

```jsonc
{
  "sim": "10:34:22", // "HH:MM:SS" bare time when dateKnown=false; ISO datetime when true
  "dateKnown": false, // true if sim provides real date
  "elapsed": 412.5, // seconds since scenario start; monotonic
  "tick": 1650, // frame counter; increments each emit
}
```

### 5.2 `diagram`

Permissive: adapters fill what the sim natively knows. Heuristic derivation is NOT prescribed; consumers may compute derived values locally if they want.

```jsonc
{
  "trainNumber": "1234A", // string | null : TC diaName; BVE: parsed from ScenarioInfo.Title
  "boundFor": "館浜", // string | null : TC native; BVE: title-parse if possible
  "serviceType": "普通", // string | null : TC native; BVE: title-keyword match
  "direction": null, // 'Upbound' | 'Downbound' | null: LineDirection
  "runNumber": null, // string | null : sim-native only; not derived
}
```

Consumers compute "remaining distance to terminus" as `stations.list[last].fromStartDistance - physics.fromStartDistance` when needed.

### 5.3 `stations`

```jsonc
{
  "list": [
    {
      "index": 0,
      "name": "起点",
      "fromStartDistance": 0, // meters from scenario start; always present
      "absoluteDistance": 35403.2, // meters | null: absolute kilometer-post (キロ程); TC always null
      "doorSide": 1, // int: -1=Left, 0=None/Unknown, 1=Right, 2=Both
      "stopType": "PassengerStop", // 'PassengerStop' | 'OperationStop' | 'Passing' | null
      "arrival": null,
      "departure": "10:00:00",
      "stopPositionName": "下り1番線", // string | null
      "isTimeTaken": true, // bool | null: timing point (採時駅); null when sim doesn't model it
      "stopPositions": [3, 4, 6], // number[] | null: candidate stop-marker car-counts for the current direction/platform; null when unknown
    },
    // ... per station
  ],
  "currentIndex": null, // number | null: station the train is currently at
  "nextIndex": 5, // number | null: next station ahead
}
```

`name` is the station's display name **only** — no station codes or numbering (e.g. `"品川"`, never `"KK01 品川"`, `"品川(JK20)"`, or `"KK01"`). Like all strings it is emitted as literal UTF-8 with no `\u` escape sequences (see the String encoding note in §3.1).

`isTimeTaken`: bool | null: timing point (採時駅); null when the sim doesn't model it. Producers that derive this heuristically SHOULD report `false` rather than `null` when time data is present but no real arrival/departure applies at this station.

Consumers derive full station records + live distance to next via lookup:

```js
const next =
  stations.nextIndex != null ? stations.list[stations.nextIndex] : null;
const distanceToNext =
  next != null ? next.fromStartDistance - physics.fromStartDistance : null;
```

### 5.4 `physics`

```jsonc
{
  "speed": 78.4, // km/h; train-level; always present
  "fromStartDistance": 12345.6, // meters traveled from scenario start point; always present
  "absoluteDistance": 47823.6, // meters | null: absolute kilometer-post position on the route (キロ程)
  "gradient": null, // ‰ | null: BVE 2.0.8 doesn't expose
  "mrPressure": 740.0, // kPa; train-level; always present
}
```

- `fromStartDistance` is always present: meters traveled since the scenario started. Monotonically increasing during normal operation (decreasing only when the train reverses).
- `absoluteDistance` is the official surveyed kilometer-post position (キロ程). Useful for cross-route correlation, ATS beacon lookup, and lat-lon mapping. Nullable when the sim only knows scenario-relative distance.

Per-car BC pressure and amperage live in `cars`.

### 5.5 `controllers`

```jsonc
{
  "powerNotch": 2, // TC Pnotch/BVE Handles.PowerNotch
  "brakeNotch": 0, // TC Bnotch/BVE Handles.BrakeNotch
  "reverser": 1, // int: -1=Reverse, 0=Neutral, 1=Forward
  "ato": null, // { active: bool, notch?: number } | null
  "tasc": null, // { active: bool, notch?: number, inching: bool } | null
  "deadman": null, // 'Hand' | 'Foot' | 'EB' | null: which channel is currently engaged
}
```

- `ato` is non-null when an Automatic Train Operation system is engaged on the train. `notch` (optional) is the notch ATO is asserting.
- `tasc` (定位置停止装置) is non-null when Train Automatic Stop Control is engaged for the upcoming stop. `active` is true when TASC is in control of brake assertion; `inching` is true when TASC is in its final low-speed alignment phase (small repeated adjustments to align with the platform stop marker). `notch` (optional) is the notch TASC is asserting.

Note: `reverser` is an int with convention `-1 = Reverse, 0 = Neutral, 1 = Forward`. TC natively emits values outside this range (e.g. `-2` is a brake-priority-selector toggle that isn't a real-train reverser feature): Rudolf adapters MUST clamp non-standard values to the nearest natural reverser position (or omit/keep last value) rather than passing through. Consumers MAY assume reverser is always in `{-1, 0, 1}`.

### 5.6 `doors`

```jsonc
{
  "allClosed": true,
  "perCar": [
    { "carNo": 1, "sideOpened": 0 },
    { "carNo": 2, "sideOpened": 0 },
    { "carNo": 3, "sideOpened": 1 }, // right side open on this car
    { "carNo": 4, "sideOpened": 3 }, // open, side unknown (e.g. TRAIN CREW)
    // ...
  ],
}
```

`sideOpened` is `int | null`. Convention mirrors `stations.list[].doorSide` but adds a positive value (`3`) for "open, side unknown," and reserves `null` for the spec-wide "no value" meaning (§3.1):

- `-1` = Left side open
- `0` = Closed (all doors on this car are shut: known closed state)
- `1` = Right side open
- `2` = Both sides open
- `3` = Open, side unknown (sim knows doors are open but can't distinguish L/R)
- `null` = No per-car door value available (spec §3.1)

Notes:

- `allClosed` (train-level bool) stays first-class: it's natively provided by both sims (`TC TrainState.AllClose`, `BVE DoorSet.AreAllClosed`) and is the load-bearing "safe to proceed?" indicator for the HMI.
- TC has only one bool per car (no L/R distinction): TC adapter emits `0` when closed and `3` (open, side unknown) when open (since the side cannot be determined natively). BVE adapter emits real per-side data (`-1`/`1`/`2`).

### 5.7 `lamps`

```jsonc
{
  "values": {
    "doorClose": 1,
    "atsReady": 1,
    "atsBrakeApply": 0,
    "regenerative": 1,
    "pilot": 1,
    // sim/vehicle-specific keys allowed freely
  },
}
```

**State convention:**

- `0` = off
- `1` = on
- `2+` = vehicle-specific alternative states (blinking, dim, multicolor, …); UI may or may not respect them. Basic HMI that only knows 0/1 SHOULD treat any nonzero as truthy.

**Default vocabulary** (consumers should know these): `doorClose, atsReady, atsBrakeApply, atsOpen, regenerative, ebTimer, emergencyBrake, overload, pilot, ato`.

**BVE-specific:** `AtsPanelArray[1024]` (vehicle-author convention) is mapped to named keys via `SimulatorProfile.vocabularies.lamps.bveIndexToKey` before emit. Raw array may additionally appear in `extensions["bve:atsPanelArray"]` for advanced consumers (per-vehicle-plugin debuggers, etc.).

### 5.8 `ats`

```jsonc
{
  "class": "ATS-P", // string | null : TC ATS_Class; BVE: from per-family-profile (v1: usually null)
  "speed": -1, // number | null : current ATS speed limit. -1 = free (unlimited); null = blank display; otherwise km/h
  "state": "P接近", // string | null : TC ATS_State (rich); BVE v1: 'EB' or null
  "richState": null, // { code: string[], name: string[], severity: number[], type: AtsRichStateType[] } | null : parallel arrays, index N = Nth active state
}
```

`ats.speed` convention: `-1` = free (unlimited/ATS not asserting any cap), `null` = display blank (no value to show), any other number = the asserted speed cap in km/h. This replaces TC's "F"-mapped-to-magic-`300` hack and the previous `'free'` string sentinel: all values are now numeric (or null), so consumers don't need union-type handling.

**`richState` structure:** When non-null, `richState` carries four parallel arrays, `code`, `name`, `severity`, and `type`, where index N across all four describes the Nth simultaneously active ATS state. `code` is the sim's raw free-form string (e.g. `"P_APPROACH"`); `name` is the display label (e.g. `"P接近"`); `severity` is `0` (info) / `1` (warning) / `2` (critical), with values above `2` reserved for sim/vehicle-specific custom severities; `type` is the machine-readable category from the vocabulary below.

**`AtsRichStateType` vocabulary:**

| Value           | Japanese Terminology   | Meaning                                                                                                                       |
| --------------- | ---------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| `SpeedCheck`    | 速度照査               | Flat continuous ceiling speed check; no falling pattern active. Default cruising state when ATS enforces a fixed speed limit. |
| `SignalP`       | 信号パターン           | Falling pattern enforced on a restrictive or stop signal (block, home, or departure signal).                                  |
| `CurveP`        | C信号（京急など）      | Falling pattern enforced on a curve or turnout speed restriction.                                                             |
| `TerminalP`     | 終端パターン           | Falling pattern protecting a track end or overrun-sensitive siding entry.                                                     |
| `PApproach`     | P接近                  | Warning that a falling pattern is being approached; pattern not yet established.                                              |
| `AckPending`    | 確認扱い               | Chime sounding; awaiting driver confirmation before EB applies. Common for ATS-S.                                             |
| `BApplication`  | 常用ブレーキ動作       | Service brake applied by the system (non-emergency).                                                                          |
| `EbApplication` | 非常ブレーキ動作       | Emergency brake applied by the system.                                                                                        |
| `StopP`         | 停車パターン・停通防止 | Falling pattern preventing station overruns and accidental signal passes.                                                     |
| `NotchCut`      | ノッチカット           | Traction power cut off enforced by the system.                                                                                |
| `BIsolated`     | 保安装置開放           | Safety device cut out or isolated by the driver.                                                                              |
| `Failure`       | 故障                   | Fault or error condition reported by the safety device.                                                                       |
| `ModeSelect`    | ATS/ATC切替            | Notice to switch over safety system (ATS/ATC changeover, line ruleset switch, or depot/test mode activation).                 |
| `Other`         | -                      | Unclassified or sim-specific state not covered by the above.                                                                  |

### 5.9 `signals`

```jsonc
{
  "list": [
    {
      "name": "三田場内", // string | null : TC native; BVE: synthesized "SecXXXm"
      "type": "Home", // 'Block' | 'Distant' | 'CallOn' | 'Shunt' | 'Home' | 'Departure' | null
      "phase": 3, // number | null : int with vocabulary (below); 3 = Y here
      "distance": 412, // meters
      "transponders": [
        {
          "category": "Pattern", // see vocabulary below; null = uncategorized
          "code": 1003, // number | null: sim-native beacon type code (BVE Beacon.Type); null if sim doesn't expose
          "speedLimit": 65, // number | null: km/h imposed by this transponder, if applicable
          "distance": 412, // meters; negative = already passed
        },
      ],
    },
  ],
}
```

`list` is ordered **nearest-first** (ascending `distance`), so `list[0]` is the closest signal ahead of the train.

**Default transponder category vocabulary:**

| Category  | Meaning                                                                                             |
| --------- | --------------------------------------------------------------------------------------------------- |
| `Pattern` | Pattern source transponder (ATS-P/ATS-Pn pattern; speed limit derived from upcoming signal aspect). |
| `Signal`  | Signal transponder (ATS-S, SN, SW conveying R/Y/G info to the cab).                                 |
| `TASC`    | TASC/stop-position marker (read by TASC for platform alignment).                                    |
| `Other`   | Recognized but doesn't fit any specific category.                                                   |
| `null`    | Uncategorized/unknown.                                                                              |

`category` is what HMI UIs switch-case on for rendering. `code` is the sim-native int (e.g. BVE's `Beacon.Type`); preserved verbatim so advanced consumers can do exact-match lookups. `SimulatorProfile.vocabularies.transponders` maps `code` → human-readable string when the sim/route author has registered meanings. Adapters that can't categorize MUST emit `category: null` rather than guessing: leave inference to consumers that have route-specific knowledge.

**Default signal phase vocabulary:**

| Index | Code                   | Japanese    | Meaning                                                                                         |
| ----- | ---------------------- | ----------- | ----------------------------------------------------------------------------------------------- |
| 0     | :                      | :           | Disabled/broken/no signal info available (distinct from R, which is a deliberate "stop" aspect) |
| 1     | R                      | 停止        | Stop                                                                                            |
| 2     | YY                     | 警戒        | Restricted/Warning (~25 km/h)                                                                   |
| 3     | Y                      | 注意        | Caution (~45 km/h)                                                                              |
| 4     | YG                     | 減速        | Reduced (~65 km/h)                                                                              |
| 5     | YGF                    | 抑速/YG点滅 | YG flashing/yokusoku (Keikyu, Keisei, ~75-105 km/h)                                             |
| 6     | G                      | 進行        | Proceed (line speed)                                                                            |
| 7     | GG                     | 高速進行    | High-speed proceed (Hokuetsu, Shinkansen)                                                       |
| 8+    | (sim/vehicle-specific) |             | Per SimulatorProfile.vocabularies.signalPhase                                                   |

Indexing rationale: `0` is reserved for "disabled/unknown/no signal info" so consumers can distinguish a _non-functional_ signal from a deliberate R aspect (which is a real instruction, not the absence of one). Within the proceed-ordered range `1..7`, indices increase with increasing permissiveness: each higher number is at-least-as-permissive as the previous, with YGF correctly slotted between YG (65 km/h) and G (line speed) since it grants 75-105 km/h on the private railways that use it.

BVE adapter MUST add `+1` to `Section.CurrentSignalIndex` when emitting (BVE's native `0=R` becomes Rudolf's `1=R`, etc.). The 15-route corpus survey validated BVE's native indices 0-4, which under Rudolf transposition become 1-5. Routes using non-default meanings (e.g. a route emitting BVE-native `4` to mean YGF rather than G) override via `SimulatorProfile.vocabularies.signalPhase`.

**Default signal-phase-speed table:**

| Phase index | Default km/h | Notes                                                 |
| ----------: | -----------: | ----------------------------------------------------- |
|           0 |         `-1` | Disabled/unknown; no cap to display                   |
|           1 |          `0` | R (stop)                                              |
|           2 |         `25` | YY (~25 km/h)                                         |
|           3 |         `45` | Y (~45 km/h)                                          |
|           4 |         `65` | YG (~65 km/h)                                         |
|           5 |         `90` | YGF (centre of 75–105 km/h range)                     |
|           6 |         `-1` | G (no inherent cap; line speed)                       |
|           7 |         `-1` | GG (no inherent cap; high-speed proceed)              |
|          8+ | (no default) | Producers publish via `vocabularies.signalPhaseSpeed` |

**Value convention for `vocabularies.signalPhaseSpeed`:**

- `n ≥ 0` — the km/h speed cap imposed by this aspect.
- `-1` — unlimited (no inherent cap; line speed or route-defined maximum).
- `null` — unknown (phase exists but no speed value is available).

Consumers compute the effective phase speed via `vocab?.signalPhaseSpeed?.[String(phase)] ?? defaults[phase]`, where the `?? defaults[phase]` fallback fires only on a _missing key_, not on an explicit `null` value.

### 5.10 `speedLimit`

```jsonc
{
  "current": 90, // km/h
  "currentType": "SpeedLimit", // 'Signal' | 'SpeedLimit' | 'Restriction' | null
  "next": [
    // Array<{ limit, distance, type }> | null — upcoming changes, nearest first. null when none known.
    {
      "limit": 65,
      "distance": 412,
      "type": "Signal", // 'Signal' | 'SpeedLimit' | 'Restriction' | null
    },
    // ...further upcoming changes, when the producer knows them
  ],
}
```

`type`/`currentType` vocabulary:

- `'SpeedLimit'`: the line's posted base speed limit (permanent civil-engineering limit at this location)
- `'Signal'`: a limit imposed by an upcoming signal aspect (e.g. ATS-P pattern derived from a Y signal ahead)
- `'Restriction'`: a temporary or operational restriction (curve restriction, weather-related slow order, work zone, station-approach restriction, special-event slow)
- `null`: type unknown or unclassified (sim has the limit value but not its origin)

**`next` ordering and completeness:** `next` is an array of upcoming speed-limit changes ordered **nearest-first** (ascending `distance`), so `next[0]` is the closest change ahead. It is `null` when the sim knows of no upcoming change — never an empty array. A producer that only knows the immediate next change emits a single-element array; a producer that knows the whole forward sequence emits every upcoming change. Which of the two a producer does is declared in `SimulatorProfile.capabilities['speedLimit.next']`: `'full'` (lists all upcoming changes) | `'single'` (only the immediate next) | `false`/absent (unsupported).

### 5.11 `cars`

Per-car DYNAMIC state. Static per-car data (model, hasMotor/Cab/Pantograph, cabDirection, pantographType, pantographDirection, length) lives in `SimulatorProfile.vehicle.cars`: NOT duplicated per-frame.

```jsonc
{
  "list": [
    {
      "carNo": 1,
      "bcPressure": 307.4, // kPa | null : TC native per-car; BVE: broadcast from [0]
      "amperage": 124, // A | null   : TC native per-car; BVE: broadcast from [0]
      "occupancyRate": null, // percentage filled (may exceed 100%) | null : TC native; BVE: null
    },
    // ...
  ],
}
```

Per-car-physics realness is declared in `SimulatorProfile.capabilities['physics.perCar']`: `'true'` | `'broadcast'` | `'unavailable'`.

### 5.12 `switches`

```jsonc
{
  "hornAir": false,
  "hornElectric": false,
  "buzzerDriver": false, // driver-initiated (to conductor)
  "buzzerConductor": false, // conductor-initiated (to driver)
  "headlights": false, // bool: true when headlights are on (use `highBeam` to distinguish low/high beam)
  "highBeam": false,
  "wiper": null, // 'Off' | 'Intermittent' | 'Low' | 'High' | null
}
```

### 5.13 `gameState`

Sim/game meta-state: NOT train state.

```jsonc
{
  "screen": "MainGame", // 'MainGame' | 'Pause' | 'Loading' | 'Menu' | 'Result' | 'Title' | 'NotRunning' | 'Other'
  "crewRole": "Driver", // 'Driver' | 'Conductor' | 'Both' | 'Others' | null
  "driveMode": "Scored", // 'Scored' | 'Unscored' | 'Other' | null
  "isOneman": false, // TC native; BVE: title-parse or false default
}
```

### 5.14 Extensions

Extensions live under `extensions.<namespace>:<concern>`. The namespace is the sim id (`bve`, `traincrew`) or a vendor id; the concern names what the block contains.

Conventions:

- Each extension is a typed object with its own `v` semver
- Consumers MAY ignore unknown extensions
- Producers SHOULD NOT use extensions for things that fit a core section

Examples (defined by adapter authors, not Rudolf core):

```jsonc
"bve:beaconRing": {
  "list": [
    { "type": 1003, "passedAt": 12300.1, "data": 5, "optional": 0 }
  ]
}

"bve:atsPanelArray": {
  "raw": [0, 0, 1, 0, 1, /* ... 1019 more ... */]
}

"traincrew:ato": {
  "active": true,
  "notch": -3,
  "targetSpeed": 65
}
```

## 6. InputCommand

Consumer → sim. One command per InputCommand document (batching is an explicit future extension if needed). Producers should only send commands that the SimulatorProfile.capabilities declares supported.

```jsonc
{
  "schemaVersion": "1.0",
  "kind": "InputCommand",
  "scenarioId": "petrichor-e131-evening-2026-06-25T14:23:00Z",
  "sentAt": "2026-06-25T14:23:17.350Z",
  "sequenceNumber": 1042, // monotonic per consumer; for ordering/idempotency
  "command": {
    "kind": "SetNotch",
    "value": -2,
  },
}
```

### 6.1 Command types

All commands are discriminated by `command.kind`. The set:

| Kind            | Payload                                           | Semantics                                                                                                                                                                                                                                                                     |
| --------------- | ------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `SetNotch`      | `{ value: int, relative?: bool }`                 | Combined notch. `relative` (default `false`) = absolute: value is the combined notch (0=N, +n=Pn, -1=抑速, -2…=B1…). `relative: true` = signed step delta. Either way, `value <= -100` (sentinel `EB = -100`) is Emergency — train-agnostic, supersedes the old hardcoded -8. |
| `SetPowerNotch` | `{ value: int }`                                  | Power-only positive int.                                                                                                                                                                                                                                                      |
| `SetBrakeNotch` | `{ value: int }`                                  | Brake-only positive int.                                                                                                                                                                                                                                                      |
| `SetBrakeSAP`   | `{ kPa: double }`                                 | Electromagnetic direct brake SAP pressure target. 0-400 = service, 410 = emergency.                                                                                                                                                                                           |
| `SetReverser`   | `{ value: int }`                                  | Reverser position. `-1` = Reverse, `0` = Neutral, `1` = Forward. Values outside this range MUST be rejected.                                                                                                                                                                  |
| `SetButton`     | `{ action: string, state: bool }`                 | Generic button. `action` is a `VehicleAction` (§6.2) or `GameAction` (§6.3) name, or a custom action string. Custom/non-spec actions are unvalidated passthrough, gated by `capabilities['input.button.<action>']`.                                                           |
| `SetWiper`      | `{ state: 'Off'\|'Intermittent'\|'Low'\|'High' }` | Wiper position.                                                                                                                                                                                                                                                               |
| `SetAtoNotch`   | `{ value: int }`                                  | ATO notch suggestion. Per TC semantics: when notch > 0, applied only if manual notch is N; when notch < 0, max(manual, ato) applied.                                                                                                                                          |
| `SetDeadman`    | `{ method: 'Hand'\|'Foot'\|'EB', holding: bool }` | Deadman switch state per channel.                                                                                                                                                                                                                                             |

Producers MUST set fields described as such; OPTIONAL fields use a `default behavior` documented per-command.

> **`SetNotch` Emergency sentinel.** The reserved constant `EB = -100` (any `value <= -100`) requests Emergency regardless of `relative`. Prefer the constant over a bare literal; it is train-agnostic and supersedes the old hardcoded `-8`.
>
> **Custom `SetButton` actions.** `VehicleAction` (§6.2) and `GameAction` (§6.3) are the spec vocabularies whose members serialize to the `action` string. Actions outside that vocabulary travel through the same string field via a separate custom-action method, and are unvalidated passthrough — a sim declares support with `capabilities['input.button.<action>']`.

### 6.2 VehicleAction enum

Physical cab/train controls used with `SetButton`. Vocabulary derived from the TRAIN CREW SDK with cleaner naming. Each entry has a known semantic; sims may not support all: consult `SimulatorProfile.capabilities['input.button.<action>']`. Notch is no longer a button action — use `SetNotch` (§6.1). Renamed from the old `InputAction`: `Broadcast` → `InCarBroadcast`, `LightLow` → `HeadLightLow`.

- `EBReset`: reset the EB/deadman alarm (EB復帰)
- `GradientStart`: engage the gradient-start / anti-rollback switch (勾配起動スイッチ)
- `SafetyBrake`: safety-brake switch (保安ブレーキ)
- `SnowBrake`: snow-resistance brake switch (耐雪ブレーキ)
- `HornAir`: air horn (空気笛)
- `HornElectric`: electric horn (電気笛)
- `Buzzer`: cab buzzer (合図ブザー)
- `BoardingPrompt`: boarding-prompt buzzer (乗降促進)
- `InCarBroadcast`: in-car announcement / PA (車内放送) — was `Broadcast`
- `DoorOpenLeft`: open the left-side passenger doors (左ドア開)
- `DoorCloseLeft`: close the left-side passenger doors (左ドア閉)
- `DoorOpenRight`: open the right-side passenger doors (右ドア開)
- `DoorCloseRight`: close the right-side passenger doors (右ドア閉)
- `DoorReopen`: re-open/re-close switch after an interrupted closure (再開閉SW)
- `DoorKey`: door-switch key operation (ドアスイッチ鍵)
- `PartialDoor`: 3/4-door partial-open switch (3/4閉スイッチ)
- `DoorCut`: door cut-out switch (ドアカットSW)
- `HeadLightLow`: dim the headlight / low beam (前灯減光) — was `LightLow`
- `HeadLight`: headlight switch (前照灯SW)
- `CabinLight`: passenger-cabin light switch (客室灯SW)
- `CrewRoomLight`: crew-room light switch (乗務員室灯SW)
- `InstrumentLight`: instrument/meter light switch (計器灯SW)

### 6.3 GameAction enum

Camera/view/UI/sim-meta actions used with `SetButton`. These are optional — consumers SHOULD NOT depend on any of them being supported; consult `SimulatorProfile.capabilities['input.button.<action>']`.

**Camera / view:**

- `ExteriorView`: toggle the exterior/external view (外部視点切替)
- `DriverAlternateView`: driver alternate viewpoint
- `ConductorAlternateView`: conductor rear-confirmation view (後方確認)
- `LeftWindowView`: look out of the left window
- `RightWindowView`: look out of the right window

**Simulator UI:**

- `TogglePauseMenu`: toggle the pause menu
- `ToggleDiagramDisplay`: toggle the diagram/schedule display (スタフ表示)
- `ToggleGUI`: toggle the in-game UI (画面表示)
- `ToggleCrewDoor`: toggle the crew door
- `ToggleCrewWindow`: toggle the crew window

## 7. Wire transport

Rudolf defines the document shapes but is **transport-agnostic**.

Recommended transports:

- HTTP
- WebSocket/Socket.IO
- Shared memory (Windows)

## 8. Example payloads

### 8.1 SimulatorProfile (TRAIN CREW)

```json
{
  "schemaVersion": "1.0",
  "kind": "SimulatorProfile",
  "scenarioId": "51a35aec-d930-455f-a8fa-58f686f87254",
  "sentAt": "2026-07-02T20:18:18.3444612+00:00",
  "sim": {
    "name": "TRAIN CREW",
    "version": "",
    "adapterName": "Tanuden.Rudolf.Adapters.TrainCrew",
    "adapterVersion": "0.1.0"
  },
  "scenario": {
    "title": "777",
    "route": "",
    "author": null,
    "scenarioStartTime": "00:00:00",
    "diagramNumber": "777",
    "boundFor": "館浜",
    "serviceType": "普通"
  },
  "vehicle": {
    "name": "4300",
    "model": "",
    "operator": "",
    "cars": [
      {
        "carNo": 1,
        "model": "4300",
        "hasDriverCab": true,
        "hasConductorCab": true,
        "hasMotor": true,
        "hasPantograph": false,
        "cabDirection": "Right",
        "pantographType": null,
        "pantographDirection": null,
        "length": -1
      },
      {
        "carNo": 2,
        "model": "4300",
        "hasDriverCab": false,
        "hasConductorCab": false,
        "hasMotor": false,
        "hasPantograph": true,
        "cabDirection": null,
        "pantographType": null,
        "pantographDirection": null,
        "length": -1
      },
      {
        "carNo": 3,
        "model": "4300",
        "hasDriverCab": false,
        "hasConductorCab": false,
        "hasMotor": false,
        "hasPantograph": false,
        "cabDirection": null,
        "pantographType": null,
        "pantographDirection": null,
        "length": -1
      },
      {
        "carNo": 4,
        "model": "4300",
        "hasDriverCab": true,
        "hasConductorCab": true,
        "hasMotor": true,
        "hasPantograph": false,
        "cabDirection": "Left",
        "pantographType": null,
        "pantographDirection": null,
        "length": -1
      }
    ],
    "leadCar": 4,
    "capabilities": {
      "masconType": "OneHandle",
      "masconBrakeType": "Notched",
      "powerNotches": 5,
      "brakeNotches": 8,
      "ebNotch": -8,
      "holdingBrakeNotches": 0,
      "cpStartPressure": 750,
      "cpStopPressure": 880
    }
  },
  "capabilities": {
    "physics.gradient": true,
    "physics.perCar": "true",
    "ats.richState": "rich",
    "speedLimit.next": "single",
    "input.command.SetNotch": true,
    "input.command.SetPowerNotch": true,
    "input.command.SetBrakeNotch": true,
    "input.command.SetBrakeSAP": true,
    "input.command.SetReverser": true,
    "input.command.SetButton": true,
    "input.command.SetWiper": true,
    "input.command.SetAtoNotch": true,
    "input.command.SetDeadman": true
  },
  "vocabularies": {
    "lamps": null,
    "signalPhase": null,
    "transponders": null,
    "signalPhaseSpeed": null
  }
}
```

### 8.2 OutputDataFrame (TRAIN CREW)

```json
{
  "schemaVersion": "1.0",
  "kind": "OutputDataFrame",
  "scenarioId": "51a35aec-d930-455f-a8fa-58f686f87254",
  "sentAt": "2026-07-02T20:19:26.6283871+00:00",
  "time": {
    "sim": "07:51:50",
    "dateKnown": false,
    "elapsed": 28310.468,
    "tick": 639186203666283802
  },
  "diagram": {
    "trainNumber": "777",
    "boundFor": "館浜",
    "serviceType": "普通",
    "direction": "Downbound",
    "runNumber": "76"
  },
  "stations": {
    "list": [
      {
        "index": 0,
        "name": "日野森",
        "fromStartDistance": 0,
        "absoluteDistance": null,
        "doorSide": -1,
        "stopType": "PassengerStop",
        "arrival": null,
        "departure": "07:42:00",
        "stopPositionName": "日野森駅1番下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 1,
        "name": "高見沢",
        "fromStartDistance": 1764.1009521484375,
        "absoluteDistance": null,
        "doorSide": 1,
        "stopType": "PassengerStop",
        "arrival": "07:44:15",
        "departure": "07:48:30",
        "stopPositionName": "高見沢駅2番下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 2,
        "name": "水越",
        "fromStartDistance": 3536.20703125,
        "absoluteDistance": null,
        "doorSide": -1,
        "stopType": "PassengerStop",
        "arrival": "07:50:45",
        "departure": "07:51:15",
        "stopPositionName": "水越駅2番下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 3,
        "name": "藤江",
        "fromStartDistance": 4958.06591796875,
        "absoluteDistance": null,
        "doorSide": 1,
        "stopType": "PassengerStop",
        "arrival": "07:52:55",
        "departure": "07:53:25",
        "stopPositionName": "藤江駅2番下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 4,
        "name": "大道寺",
        "fromStartDistance": 7091.56201171875,
        "absoluteDistance": null,
        "doorSide": 1,
        "stopType": "PassengerStop",
        "arrival": "07:56:50",
        "departure": "08:02:00",
        "stopPositionName": "大道寺駅4番下り_併B",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 5,
        "name": "江ノ原信号場",
        "fromStartDistance": 7696.173828125,
        "absoluteDistance": null,
        "doorSide": 1,
        "stopType": "Passing",
        "arrival": "08:02:45",
        "departure": "08:02:45",
        "stopPositionName": "江ノ原信号場下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 6,
        "name": "江ノ原",
        "fromStartDistance": 8710.1552734375,
        "absoluteDistance": null,
        "doorSide": -1,
        "stopType": "PassengerStop",
        "arrival": "08:03:50",
        "departure": "08:04:20",
        "stopPositionName": "江ノ原駅下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 7,
        "name": "新野崎",
        "fromStartDistance": 10100.48046875,
        "absoluteDistance": null,
        "doorSide": -1,
        "stopType": "PassengerStop",
        "arrival": "08:06:05",
        "departure": "08:06:35",
        "stopPositionName": "新野崎駅3番下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 8,
        "name": "新井川",
        "fromStartDistance": 11253.4599609375,
        "absoluteDistance": null,
        "doorSide": -1,
        "stopType": "PassengerStop",
        "arrival": "08:08:00",
        "departure": "08:08:30",
        "stopPositionName": "新井川駅下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 9,
        "name": "羽衣橋",
        "fromStartDistance": 12402.4404296875,
        "absoluteDistance": null,
        "doorSide": -1,
        "stopType": "PassengerStop",
        "arrival": "08:10:00",
        "departure": "08:10:30",
        "stopPositionName": "羽衣橋駅下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 10,
        "name": "浜園",
        "fromStartDistance": 13540.25,
        "absoluteDistance": null,
        "doorSide": -1,
        "stopType": "PassengerStop",
        "arrival": "08:11:55",
        "departure": "08:12:25",
        "stopPositionName": "浜園駅下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 11,
        "name": "津崎",
        "fromStartDistance": 15027.8701171875,
        "absoluteDistance": null,
        "doorSide": 1,
        "stopType": "PassengerStop",
        "arrival": "08:14:20",
        "departure": "08:19:00",
        "stopPositionName": "津崎駅4番下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 12,
        "name": "虹ケ浜",
        "fromStartDistance": 17002.970703125,
        "absoluteDistance": null,
        "doorSide": -1,
        "stopType": "PassengerStop",
        "arrival": "08:21:05",
        "departure": "08:21:35",
        "stopPositionName": "虹ケ浜駅下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 13,
        "name": "海岸公園",
        "fromStartDistance": 18963.630859375,
        "absoluteDistance": null,
        "doorSide": -1,
        "stopType": "PassengerStop",
        "arrival": "08:23:30",
        "departure": "08:24:00",
        "stopPositionName": "海岸公園駅下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 14,
        "name": "河原崎",
        "fromStartDistance": 20263.310546875,
        "absoluteDistance": null,
        "doorSide": -1,
        "stopType": "PassengerStop",
        "arrival": "08:25:35",
        "departure": "08:26:05",
        "stopPositionName": "河原崎駅下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 15,
        "name": "駒野",
        "fromStartDistance": 21295.369140625,
        "absoluteDistance": null,
        "doorSide": -1,
        "stopType": "PassengerStop",
        "arrival": "08:27:30",
        "departure": "08:28:00",
        "stopPositionName": "駒野駅3番下り",
        "isTimeTaken": null,
        "stopPositions": null
      },
      {
        "index": 16,
        "name": "館浜",
        "fromStartDistance": 23008.150390625,
        "absoluteDistance": null,
        "doorSide": 1,
        "stopType": "PassengerStop",
        "arrival": "08:30:55",
        "departure": null,
        "stopPositionName": "館浜駅3番下り",
        "isTimeTaken": null,
        "stopPositions": null
      }
    ],
    "currentIndex": null,
    "nextIndex": 3
  },
  "physics": {
    "speed": 55.36149978637695,
    "fromStartDistance": 3703.76904296875,
    "absoluteDistance": 19408.52734375,
    "gradient": -1.9993319511413574,
    "mrPressure": 695.1132202148438
  },
  "controllers": {
    "powerNotch": 5,
    "brakeNotch": 0,
    "reverser": 1,
    "ato": null,
    "tasc": null,
    "deadman": null
  },
  "doors": {
    "allClosed": true,
    "perCar": [
      {
        "carNo": 1,
        "sideOpened": 0
      },
      {
        "carNo": 2,
        "sideOpened": 0
      },
      {
        "carNo": 3,
        "sideOpened": 0
      },
      {
        "carNo": 4,
        "sideOpened": 0
      }
    ]
  },
  "lamps": {
    "values": {
      "doorClose": 1,
      "atsReady": 1,
      "atsBrakeApply": 0,
      "atsOpen": 0,
      "regenerative": 0,
      "ebTimer": 0,
      "emergencyBrake": 0,
      "overload": 0
    }
  },
  "ats": {
    "class": "普通",
    "speed": 110,
    "state": null,
    "richState": null
  },
  "signals": {
    "list": [
      {
        "name": "下り閉塞193",
        "type": "Block",
        "phase": 6,
        "distance": 62.89793014526367,
        "transponders": [
          {
            "category": null,
            "code": null,
            "speedLimit": 0,
            "distance": 14.965310096740723
          },
          {
            "category": null,
            "code": null,
            "speedLimit": 0,
            "distance": 54.96318817138672
          },
          {
            "category": null,
            "code": null,
            "speedLimit": 30,
            "distance": -35.03458023071289
          }
        ]
      }
    ]
  },
  "speedLimit": {
    "current": 100,
    "currentType": "SpeedLimit",
    "next": null
  },
  "cars": {
    "list": [
      {
        "carNo": 1,
        "bcPressure": 0,
        "amperage": 702.1439208984375,
        "occupancyRate": 100
      },
      {
        "carNo": 2,
        "bcPressure": 0,
        "amperage": 0,
        "occupancyRate": 65.47618865966797
      },
      {
        "carNo": 3,
        "bcPressure": 0,
        "amperage": 0,
        "occupancyRate": 77.38095092773438
      },
      {
        "carNo": 4,
        "bcPressure": 0,
        "amperage": 702.1439208984375,
        "occupancyRate": 85.71428680419922
      }
    ]
  },
  "switches": {
    "hornAir": false,
    "hornElectric": false,
    "buzzerDriver": false,
    "buzzerConductor": false,
    "headlights": false,
    "highBeam": false,
    "wiper": null
  },
  "gameState": {
    "screen": "Pause",
    "crewRole": "Driver",
    "driveMode": "Scored",
    "isOneman": false
  },
  "extensions": {
    "traincrew:trackCircuits": {
      "list": [...]
    },
    "traincrew:otherTrains": {
      "list": [...]
    },
    "traincrew:signals": {
      "list": [...]
    },
    "traincrew:interlocking": {
      "list": [...]
    }
  }
}
```

### 8.3 InputCommand examples

```jsonc
// brake notch to B3
{ "schemaVersion": "1.0", "kind": "InputCommand", "scenarioId": "...", "sentAt": "...", "sequenceNumber": 1042,
  "command": { "kind": "SetBrakeNotch", "value": 3 } }

// air horn press
{ "schemaVersion": "1.0", "kind": "InputCommand", "scenarioId": "...", "sentAt": "...", "sequenceNumber": 1043,
  "command": { "kind": "SetButton", "action": "HornAir", "state": true } }

// air horn release
{ "schemaVersion": "1.0", "kind": "InputCommand", "scenarioId": "...", "sentAt": "...", "sequenceNumber": 1044,
  "command": { "kind": "SetButton", "action": "HornAir", "state": false } }
```
