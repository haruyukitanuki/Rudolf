# Rudolf仕様書

[English](./rudolf-spec.md) | **日本語**

## 1. 用語定義

| 用語 | 意味 |
| --- | --- |
| Sim（シム） | Rudolfドキュメントを出力する列車運転シミュレーター（BVE、TRAIN CREWなど）。 |
| Adapter（アダプター） | シミュレーターのネイティブAPIを読み取り、通信経路上でRudolf形式へ変換するコード。 |
| Consumer（コンシューマー） | Rudolfドキュメントを読み取る側全般：HMIディスプレイ、ドライブレコーダー、Webダッシュボード、物理デバイスコントローラーなど。 |
| Producer（プロデューサー） | Rudolfドキュメントを出力する側全般：通常はシミュレーターの内部または隣で動作するアダプター。 |
| Section（セクション） | `OutputDataFrame` のトップレベルキー（例：`physics`、`signals`）。 |
| Extension（拡張） | `extensions:` 配下に置かれる、名前空間付きのシミュレーター固有・ベンダー固有ブロック（例：`bve:beaconRing`）。 |
| Scenario（シナリオ） | シナリオの読み込みから終了までの、シミュレーターの1回のプレイセッション。 |
| HMI | ヒューマンマシンインターフェース。すなわち列車情報管理装置（TIMS／INTEROS／MON等）。 |

## 2. ドキュメントの種類

Rudolfは3種類のドキュメントを定義します。いずれもJSON形式で、UTF-8エンコード、キー名はcamelCase（キャメルケース）です。

| ドキュメント | 方向 | 送信頻度 | 目的 |
| --- | --- | --- | --- |
| `SimulatorProfile` | sim → consumer | シナリオ読み込み時に1回 | 静的メタデータ、対応機能（capabilities）、語彙マップ |
| `OutputDataFrame` | sim → consumer | フレームごと（通常10 Hz／100 ms間隔） | 列車およびゲーム状態のリアルタイムスナップショット |
| `InputCommand` | consumer → sim | 入力イベント発生ごと | デバイスへの入力コマンド（ノッチ、ボタンなど） |

すべてのドキュメントは共通して次のフィールドを持ちます。

- `schemaVersion: string`：Rudolf仕様のバージョン。現行バージョンは `"1.0"`。
- `kind: 'SimulatorProfile' | 'OutputDataFrame' | 'InputCommand'`：ドキュメント種別の判別子。
- `scenarioId: string`：1つのプレイセッションに属するすべてのドキュメントを紐付ける識別子。同一セッション内では、SimulatorProfile、そのシナリオ内のすべてのOutputDataFrame、およびそれらを対象とするすべてのInputCommandで同一の `scenarioId` が使用されます。ゲーム内でロードされた現在のシナリオセッションに対して一意である限り、この値のフォーマットは任意です。
- `sentAt: string`：プロデューサー側におけるISO 8601形式のタイムスタンプ。

## 3. アーキテクチャ

### 3.1 エンベロープの規約

#### 命名規約

通信経路上（ワイヤー上）ではcamelCaseを使用します。C# のプロデューサーは `CamelCasePropertyNamesContractResolver` 等によってPascalCaseから変換します。TypeScript／JavaScriptのコンシューマーはcamelCaseのまま直接読み取ります。

#### 文字列エンコーディング

すべての文字列値はリテラルなUTF-8として出力されます —— **`\uXXXX` のようなエスケープシーケンスは使用しません**。日本語テキスト（駅名、路線名、車両名など）は通信経路上にそのまま出力されなければなりません（MUST。例：`\u7ACB\u4F1A\u5DDD` のようにエスケープするのではなく、`"立会川"` とそのまま記述）。プロデューサーは、シリアライザーのエンコーダーを適切に設定してください（例：.NETの `JavaScriptEncoder.UnsafeRelaxedJsonEscaping`）。これらのドキュメントが生のHTMLに直接埋め込まれることはないため、出力をHTMLエスケープする必要はありません。なお、コンシューマーは両方の形式を受け入れられるように実装しなければなりません（MUST。`\u` エスケープされたJSONも、デコード結果は同一の文字列になります）。

#### 単位

- 速度：**km/h**
- 圧力：**kPa**
- 距離／位置：**メートル (m)**
- 勾配：**‰**（パーミル）
- 電流：**A**（アンペア）
- 時刻：ISO 8601形式の文字列。正確な日付を持たないシミュレーターのために、`Kind=Unspecified` を許容します。

フィールドの単位が **%**（パーセント）または **‰**（パーミル）である場合、通信時の値はそれぞれ割合に100または1000を掛けた数値であることを意味します。
例：
- `physics.gradient` の単位は **‰** です。勾配が -33‰ の場合、フィールドの値は `-33.0` となります。
- `cars.list[...].occupancyRate` の単位は **%** です。乗車率が 150% の場合、フィールドの値は `150.0` となります。

#### 生の値（Raw Values）

プロデューサーは、シミュレーターからのすべての物理情報を保持した生の数値を出力すべきです（SHOULD）。データの忠実性は維持されなければならず（MUST）、情報が失われるような値の変換、クランプ（範囲制限）、改変を行ってはなりません（MUST NOT）。シミュレーター側の物理計器の制約（例：メーターの針が一方向にしか動かない等）はコンシューマー側での表示上の関心事であり、データ層の値を歪める理由にはなりません。

例として、`physics.current` は回生ブレーキまたは発電ブレーキの電流を表す場合があり、これは物理的には負の値になります。一部のシミュレーターでは、運転台の電流計の針が一方向にしか振れず運転士が文脈から符号を判別する構造であるために、これを正の値として出力することがあります。Rudolfにおいては、物理的な電流が負であるなら、フィールドの値も必ず負でなければなりません（MUST）。計器が正の値しか表示できないからという理由で `Math.Abs(current)` を出力してはなりません（MUST NOT）。物理計器やHMIを駆動するコンシューマー側が、負の値を自身の表示範囲にマッピングする責任を負います。

同様に、シミュレーター自体がネイティブで行っている場合や、データ型の安全性のために絶対に必要な場合を除き、値を「妥当な」範囲にクランプしたり、四捨五入、平滑化、補間を行ったりしてはなりません（MUST NOT）。

#### null許容フィールド

`null` が設定されたフィールドは、「シミュレーターが現在その値を実際に持っていない（未取得・不明）」ことを意味します。

JSONにフィールド自体が存在しない（省略されている）場合、それは「シミュレーターがそのフィールドをそもそもサポートしていない」ことを意味します（これは `SimulatorProfile.capabilities` でも宣言されます）。通常は出力されるものの現時点で値がないフィールドについては、プロデューサーは省略するのではなく `null` を出力すべきです（SHOULD）。

#### バージョニング

すべてのドキュメントは、エンベロープレベルに単一の `schemaVersion` を持ちます。いずれかのセクションに破壊的変更が行われた場合、`schemaVersion` を更新します。コンシューマーは、将来のマイナーバージョンで追加される未知のフィールドを許容しなければなりません（MUST。理解できるフィールドを読み取り、未知のフィールドは無視する）。

### 3.2 ドキュメント構造

```
SimulatorProfile = { schemaVersion, kind, scenarioId, sentAt, sequence, sim, scenario, vehicle, capabilities, vocabularies }

OutputDataFrame = { schemaVersion, kind, scenarioId, sentAt,
                time, diagram, stations, physics, controllers, doors,
                lamps, ats, signals, speedLimits, cars, switches, gameState,
                extensions? }

InputCommand = { schemaVersion, kind, scenarioId, sentAt, sequenceNumber, command }
```

### 3.3 拡張性

- **拡張ブロック：** `extensions.<sim>:<concern>` という名前空間を使用します。例：`bve:beaconRing`、`bve:atsPanelArray`。サードパーティは独自のブロックを自由に定義できます。
- **語彙（Vocabularies）：** デフォルトの語彙（信号現示、信号現示速度、表示灯キー、地上子タイプの意味）は本仕様で定義されています。`SimulatorProfile.vocabularies` によってシナリオごとに上書きできます（MAY）。

## 4. SimulatorProfile

シナリオ読み込み時に1回送信されます。車両変更時に再送されます。`scenarioId` および `sequence` によってキャッシュ可能です。

- `scenarioId` は新しい運転セッションが開始されたときにのみ変更されます。
- `sequence`（型 `long`）は、運転中にデータが変更されたとき（例：連結・解放、異なる線路区間での信号制限速度変更など）にインクリメントされます。

```json
{
  "schemaVersion": "1.0",
  "kind": "SimulatorProfile",
  "scenarioId": "51a35aec-d930-455f-a8fa-58f686f87254",
  "sentAt": "2026-07-02T20:18:18.3444612+00:00",
  "sequence": 1,
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
    "time.dateKnown": false,
    "physics.gradient": true,
    "physics.curveRadius": false,
    "physics.perCar": "True",
    "ats.richState": true,
    "stations.next": "MultiStatic",
    "speedLimits.next": "Single",
    "signals.next": "Single",
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

車両の静的な制御機器情報です。トップレベルの `capabilities` マップ（アダプターがどの `OutputDataFrame` フィールドを実際に生成・配信するかを宣言するもの）とは別個に定義されます。すべてのフィールドはnull許容であり、`null` は「シミュレーターが現時点で値を持たない」ことを意味します。

- `masconType`：マスコンのハンドル方式。`'OneHandle' | 'TwoHandle' | null`（MasconType）。
- `masconBrakeType`：ブレーキハンドルの動作方式。`'Notched' | 'LapCapable' | 'Continuous' | null`（MasconBrakeType）。`LapCapable` は重なり位置（ラップ）を持つ制御（連続制御を含意）、`Continuous` は重なり位置を持たない無段階（直通空気ブレーキなど）のハンドルです。
- `powerNotches`：力行ノッチ段数（例：P1〜P5なら5）。不明な場合は `null`。
- `brakeNotches`：常用ブレーキノッチ段数（例：B1〜B8なら8）。不明な場合は `null`。
- `ebNotch`：SetNotchエンコーディングにおいて非常ブレーキ（EB）を表す符号付きノッチ値（例：`-8`）。不明な場合は `null`。
- `holdingBrakeNotches`：抑速ブレーキのノッチ段数。備えていない場合は `0`、不明な場合は `null`。
- `cpStartPressure`／`cpStopPressure`：空気圧縮機（コンプレッサー）の起動／停止圧力（kPa）。不明な場合は `null`。

### 4.2 `vehicle.name`, `vehicle.model` & `vehicle.operator`

- `name`：車両形式の表示名（例：`"225系0番台"`）。「系」や「番台」の漢字表記が正確であることを確認してください。編成内に複数の形式が混結されている場合は、`+` で連結します（例：`"E231系1000番台+E233系3000番台"`）。
- `model`：車両モデル識別子（例：`"225-0"`）。相互運用性を最大化するため、`series-subseries` 形式とすべきであり（SHOULD）、かな表記はTitleCaseでローマ字化すべきです（SHOULD）。編成内に複数の形式が混結されている場合は、`+` で連結します（例：`"E231-1000+E233-3000"`）。
- `operator`：運行会社（例：`"EastJapanRailwayCompany"`、`"TokyuCorporation"`）。互換性を最大化するため、グループ名ではなく日本語版Wikipediaに準拠した正式な鉄道事業者名をTitleCaseで記述すべきです（SHOULD）。

### 4.3 `capabilities`

本セクションは、`OutputDataFrame` 内の各データフィールドがどのように設定されるか、またはそのフィールドがサポートされているかについての情報を提供します。また、シミュレーターがサポートする `InputCommand` の種類も指定します。すべてのキーは省略可能であり（OPTIONAL）、未定義のキーは非対応として扱われなければなりません（MUST）。

#### 4.3.1 OutputDataFrame Capabilities

| キー | 値 | 説明 |
| :--- | :--- | :--- |
| `time.dateKnown` | `bool` | シミュレーターが正確な実日付を提供する場合 `true`。これはプロデューサーが時刻文字列をどのように提供しなければならないかに影響します（MUST。§5.1参照）。 |
| `physics.gradient` | `bool` | 勾配データの利用可否。 |
| `physics.curveRadius` | `bool` | 曲線半径データの利用可否。 |
| `physics.perCar` | {`True`, `Broadcast`, `Unavailable`} のいずれか | 車両ごとの物理データの利用可否。`True` の場合、`DataFrame.cars` に全車両の実データが含まれます。`Broadcast` の場合、先頭車両のデータのみが存在し、コンシューマー側で先頭の値を全車にブロードキャストしなければなりません（MUST）。`Unavailable` の場合、`DataFrame.cars` に車両ごとのデータは提供されません。 |
| `ats.richState` | `bool` | `DataFrame.ats.richState` コレクションの利用可否（§5.8参照）。 |
| `stations.next` | `NextItemArrayType` | 駅データ配列の配信形態。 |
| `speedLimits.next` | `NextItemArrayType` | 速度制限データ配列の配信形態。 |
| `signals.next` | `NextItemArrayType` | 信号データ配列の配信形態。 |

`NextItemArrayType` は、シナリオ内のオブジェクトを格納する配列の動作を指定します：

| 値 | 配列の要素数 | 配列内のデータ |
| :--- | :--- | :--- |
| `None` | 0 件 | なし。 |
| `Single` | 0 または 1 件 | 列車の前方にある直近のオブジェクト、またはなし。 |
| `MultiDynamic` | 任意の件数 | 列車の前方にある複数のオブジェクト、またはなし。必ずしもシナリオ終端までとは限りません。 |
| `MultiStatic` | 任意の件数 | シナリオの開始から終了までの全項目。`stations.next` にのみ適用されます。 |

#### 4.3.2 InputCommand Capabilities

| キー | 値 | 説明 |
| :--- | :--- | :--- |
| `input.command.*` | `bool` | `*` は §6.1 で定義されるコマンド種別です。 |
| `input.button.*` | `bool` | `*` は SetButton コマンドで使用される操作対象です。標準の SetButton 操作は §6.2 および §6.3 で定義されています。 |

### 4.4 `vocabularies`

キーと値のペアによるシミュレーター固有の上書き設定です。各セクションはnull許容です：`null` は上書きが適用されず、コンシューマーが本仕様に定義されたデフォルト値へフォールバックすることを意味します。

| セクション | 変更対象 | キー | 値 |
| :--- | :--- | :--- | :--- |
| `lamps` | 表示灯名／インデックスのマッピング（§5.7参照） | `string`。テキストとしての表示灯名。 | `int`。表示灯配列のインデックス。 |
| `signalPhase` | 信号現示名（§5.9参照） | `string`。テキストとしての信号インデックス番号。 | `string`。信号コード（例："R"）。 |
| `signalPhaseSpeed` | 信号制限速度テーブル（§5.9参照） | `string`。テキストとしての信号インデックス番号。 | `double \| null`。速度（km/h）。 |
| `transponders` | 地上子カテゴリー（§5.9参照） | `string`。テキストとしてのシミュレーターネイティブのコード番号（例：BVEのBeacon.Type）。 | `string`。人間可読な名前。 |

`signalPhaseSpeed` セクションの設定例を以下に示します：

```jsonc
    "signalPhaseSpeed": {
      "1": 0,
      "2": 25,
      "3": 55,
      "4": 80,
      "6": 110
    },
```

**注意：**

- プロデューサーはすべての `vocabularies` に新しい値を追加してよく（MAY）、`signalPhaseSpeed` のデフォルト値を上書きして構いません（MAY）。
- 相互運用性を担保するため、`lamps`、`signalPhase`、および `transponders` の既存の名前を変更してはなりません（MUST NOT）。
- シナリオ内でいずれかの `vocabularies` が変更された場合、コンシューマーに再読み込みを促すために `scenarioId` も更新しなければなりません（MUST）。

## 5. OutputDataFrame

フレームごとに送信されます（通常約10 Hz／100 ms間隔ですが、シミュレーターにより増減します）。コアとなる各セクションのキーは、構造上つねに存在します（空データの場合でも保持されます）。セクション内の個別フィールドは `null` になる場合があります。

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
  "speedLimits": {
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
    // 省略可能
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
  "sim": "10:34:22", // time.dateKnown capabilityがfalseのときは "HH:MM:SS" 形式の時刻文字列、trueのときはISO日時文字列
  "elapsed": 412.5, // シナリオ開始からの経過秒数（単調増加）
  "tick": 1650, // フレームカウンター。出力ごとにインクリメント
}
```

### 5.2 `diagram`

寛容な設計（Permissive）：アダプターはシミュレーターが元々把握している情報のみを設定します。推測による値の算出は規定されません。コンシューマーが必要に応じてローカルで導出値を計算して構いません（MAY）。

```jsonc
{
  "trainNumber": "1234A", // string | null：TCの運行番号（diaName）／BVE：ScenarioInfo.Titleからパース
  "boundFor": "館浜", // string | null：TCはネイティブ値／BVE：可能ならタイトルからパース
  "serviceType": "普通", // string | null：TCはネイティブ値／BVE：タイトルのキーワード一致
  "direction": null, // 'Upbound' | 'Downbound' | null：路線方向（上り/下り）
  "runNumber": null, // string | null：シミュレーターネイティブ値のみ（自動導出しない）
}
```

コンシューマーは、必要に応じて終着駅までの残り距離を `stations.list[last].fromStartDistance - physics.fromStartDistance` として計算します。

### 5.3 `stations`

```jsonc
{
  "list": [
    {
      "index": 0,
      "name": "中京",
      "fromStartDistance": 0, // シナリオ開始地点からのメートル数（必須）
      "absoluteDistance": 35403.2, // meters | null：絶対キロ程
      "doorSide": 1, // int：開扉方向（§5.6参照）。左右判定不能時は3
      "stopType": "PassengerStop", // 'PassengerStop' (客扱い) | 'OperationStop' (運転停車) | 'Passing' (通過) | null
      "arrival": null,
      "departure": "10:00:00",
      "stopPositionName": "下り1番線", // string | null
      "isTimeTaken": true, // bool | null：採時駅かどうか。シミュレーターが未対応の場合はnull
      "stopPositions": [3, 4, 6], // number[] | null：現在の方向／番線における停止目標の両数候補。不明時はnull
    },
    // ... 駅ごとに繰り返し
  ],
  "currentIndex": null, // number | null：列車が現在停車している駅のインデックス
  "nextIndex": 5, // number | null：前方の次駅のインデックス
}
```

`name` は駅の表示名**のみ**でなければならず（MUST）、駅ナンバリングや駅コードを含めてはなりません（MUST NOT。例：`"品川"`。`"KK01 品川"`、`"品川(JK20)"`、`"KK01"` などは不可）。他のすべての文字列と同様に、`\u` エスケープシーケンスを使用しないリテラルUTF-8として出力されます（§3.1「文字列エンコーディング」参照）。

`doorSide` は §5.6 の車両ごとのドアと共有される `SideOpened` 整数規約を使用し、決して `null` にはなりません：左右の側を判定できないプロデューサーは `3`（開扉・側別不明）を出力しなければなりません（MUST）。プロデューサーは、たとえ `0`（閉扉）と `3` に限定される場合でも、ヒューリスティックにこれを導出して構いません（MAY）。

`isTimeTaken`：採時駅かどうか（bool | null）。シミュレーターが対応していない場合は `null` です。ヒューリスティックに導出するプロデューサーは、時刻データが存在するものの有効な着・発時刻が適用されない駅に対しては、`null` ではなく `false` を出力すべきです（SHOULD）。

コンシューマーは、参照（ルックアップ）によって完全な駅レコードと次駅までのリアルタイム距離を導出します。

```js
const next =
  stations.nextIndex != null ? stations.list[stations.nextIndex] : null;
const distanceToNext =
  next != null ? next.fromStartDistance - physics.fromStartDistance : null;
```

### 5.4 `physics`

```jsonc
{
  "speed": 78.4, // km/h。列車全体の速度（必須）
  "fromStartDistance": 12345.6, // シナリオ開始地点からの走行距離（メートル、必須）
  "absoluteDistance": 47823.6, // meters | null：路線上の絶対キロ程
  "curveRadius": -500.0, // meters | null：TRAIN CREWは非公開。左カーブは負、右カーブは正、直線は0
  "gradient": null, // ‰ | null：旧バージョンのBVEEx等は非公開
  "mrPressure": 740.0, // kPa。元空気溜圧力（必須）
}
```

- `fromStartDistance` は必須フィールドです。シナリオ開始からの累計走行距離（メートル）を表します。通常の運転中は単調増加します（後退時のみ減少）。
- `absoluteDistance` は公式に測量されたキロ程です。複数路線間のデータ連携、ATS地上子の参照、位置情報マッピング等に役立ちます。シミュレーターがシナリオ相対の距離しか持たない場合は `null` になります。
- `curveRadius` および `gradient` は先頭車両の位置における正確な値であるべきです（SHOULD）。正確な値が得られない場合は、キーフレーム値の使用が許可されます（MAY）。

車両ごとのBC圧力（ブレーキシリンダー圧力）および電流値は `cars` に格納されます。

### 5.5 `controllers`

```jsonc
{
  "powerNotch": 2, // TCのPノッチ / BVEのHandles.PowerNotch
  "brakeNotch": 0, // TCのBノッチ / BVEのHandles.BrakeNotch
  "reverser": 1, // int：-1=後進、0=中立、1=前進
  "ato": null, // { active: bool, notch?: number } | null
  "tasc": null, // { active: bool, notch?: number, inching: bool } | null
  "deadman": null, // 'Hand' | 'Foot' | 'EB' | null：現在作動している方式
}
```

- `ato` は、列車で自動列車運転装置（ATO）が作動している場合に非nullになります。`notch`（省略可能）はATOが指示しているノッチ段数です。
- `tasc`（定位置停止装置）は、次の停車に向けてTASCが作動している場合に非nullになります。`active` はTASCがブレーキ出力を制御している場合にtrueとなり、`inching` はTASCが最終の低速位置合わせ段階（停止目標に合わせる小刻みな微調整）にある場合にtrueとなります。`notch`（省略可能）はTASCが指示しているノッチ段数です。

注意：`reverser` は `-1 = 後進、0 = 中立、1 = 前進` という規約のintです。TRAIN CREWはこの範囲外の値をネイティブに出力することがあります（例：`-2` は実車の逆転器には存在しないブレーキ優先セレクターの切り替え）。Rudolfのアダプターは、こうした非標準の値をそのまま通すのではなく、最も近い本来の逆転器位置へクランプしなければなりません（MUST。または省略／直前の値を保持）。コンシューマーは、レバーサー位置がつねに `{-1, 0, 1}` のいずれかであると仮定してよいものとします（MAY）。

### 5.6 `doors`

```jsonc
{
  "allClosed": true,
  "perCar": [
    { "carNo": 1, "sideOpened": 0 },
    { "carNo": 2, "sideOpened": 0 },
    { "carNo": 3, "sideOpened": 1 }, // この車両は右側が開いている
    { "carNo": 4, "sideOpened": 3 }, // 開扉中だが側別不明（例：TRAIN CREW）
    // ...
  ],
}
```

`sideOpened` は `int | null` 型です。`stations.list[].doorSide` と同一の `SideOpened` 値空間（`3` = 開扉・側別不明を含む）を使用しますが、`doorSide` とは異なり `null` になる場合があり、仕様共通の「値なし」の意味（§3.1）に予約されています：

- `-1` = 左側開扉
- `0` = 閉扉（この車両のすべてのドアが閉じていることが確定している状態）
- `1` = 右側開扉
- `2` = 両側開扉
- `3` = 開扉中（側別不明：ドアが開いていることは把握できるが左右を区別できない場合）
- `null` = 車両ごとのドア値が存在しない（仕様§3.1）

補足：

- `allClosed`（列車全体の閉扉状態を示すbool）は重要な指標として維持されます：両シミュレーターがネイティブに提供しており（`TC TrainState.AllClose`、`BVE DoorSet.AreAllClosed`）、HMIにおいて「出発可能か」を判断する要となるインジケーターです。
- TRAIN CREWは車両ごとに1つの真偽値（bool）しか持ちません（左右の区別なし）：TCアダプターは、閉扉時に `0` を、開扉時に `3`（開扉中・側別不明）を出力します（ネイティブで側別判定が不可能なため）。BVEアダプターは、実際の側別データ（`-1`／`1`／`2`）を出力します。

### 5.7 `lamps`

表示灯（ランプ）には、主に単純な状態インジケーターを目的としたデータが格納されます。最大512スロットが利用可能で、そのうち128スロットには定義済みの意味があるか、予約されています。プロデューサーは `vocabularies` を使用してさらに多くの表示灯を定義できます（MAY）。コンシューマーはインデックスを使用して表示灯にアクセスし、`vocabularies` を使用して名前からインデックスへ変換できます。

```jsonc
{
  "values": [
    1, 1, 0, 0, // ... 合計512個の値
  ],
}
```

**状態の規約：**

- `0` = 消灯 (off)
- `1` = 点灯 (on)
- `2`以上 = 車両固有の代替状態（点滅、減光、多色など）。UIはこれらを解釈してもしなくても構いません（MAY）。0／1のみに対応するシンプルなHMIは、0以外をすべて「点灯（真）」として扱うべきです（SHOULD）。

**デフォルト語彙**（コンシューマーが事前に知っておくべきキー）：`doorClose, atsReady, atsBrakeApply, atsOpen, regenerative, ebTimer, emergencyBrake, overload, ato, snowBrake, wheelSlip`。

| インデックス | 名前 | 意味 |
| :--- | :--- | :--- |
| 0 | `doorClose` | 全てのドアが閉まっている一般的な状態（戸閉）。 |
| 1 | `atsReady` | ATS正常作動中（ATS投入／正常）。 |
| 2 | `atsBrakeApply` | ATSブレーキ動作中。 |
| 3 | `atsOpen` | ATS開放（無効化）。 |
| 4 | `regenerative` | 回生ブレーキ作動中。 |
| 5 | `ebTimer` | EB装置（警報／デッドマン）警告中。 |
| 6 | `emergencyBrake` | 非常ブレーキ作動中。 |
| 7 | `overload` | 電気的過負荷・過電流障害（過負荷）。 |
| 8 | `ato` | 自動列車運転装置（ATO）。 |
| 9 | `snowBrake` | 耐雪ブレーキ作動中。 |
| 10 | `wheelSlip` | 空転・滑走。 |
| 11..127 | (null) | 予約領域。 |
| 128..511 | (null) | プロデューサーが自由に定義可能（MAY）。 |

**BVE固有：** 車両プラグインデバッガー等の高度なコンシューマー向けに、1024個の整数からなる生の配列が `extensions["bve:atsPanelArray"]` に提供される場合があります（MAY）。

### 5.8 `ats`

```jsonc
{
  "class": "ATS-P", // string | null：TCのATS_Class／BVE：ファミリーごとのプロファイルから（v1では通常null）
  "speed": -1, // number | null：現在のATS照査速度。-1 = 照査なし（無制限）／null = 表示器消灯／それ以外はkm/h数値
  "state": "P接近", // string | null：TCのATS_State（リッチ文字列）／BVE v1：'EB' またはnull
  "richState": [], // AtsRichState[]：現在アクティブなATS状態オブジェクトの配列
}
```

`ats.speed` の規約：`-1` は照査なし（無制限／ATSが上限を課していない状態）、`null` は表示器消灯（表示する値がない状態）、それ以外の数値は課されている照査速度（km/h）を表します。これは、TRAIN CREWにおける "F"（照査なし）をマジックナンバー `300` にマッピングするハックや、以前の `'free'` という文字列センチネルを置き換えるものです。すべての値が数値（またはnull）となったため、コンシューマー側でユニオン型の処理を行う必要がなくなりました。

**`richState` の構造：** `richState` は `AtsRichState` オブジェクトの配列を持ちます。各 `AtsRichState` オブジェクトは現在アクティブなATS状態を表し、`code`、`name`、`severity`、および `type` の各フィールドを含みます。`code` はシミュレーターの生の自由形式文字列（例：`"P_APPROACH"`）、`name` は表示ラベル（例：`"P接近"`）、`severity` は `0`（情報）／`1`（警告）／`2`（重大）で、`2` を超える値はシミュレーター・車両固有のカスタム重大度に予約されています。`type` は以下の語彙による機械可読なカテゴリーです。

**`AtsRichStateType` 語彙：**

| 値 | 日本語表現 | 意味 |
| --- | --- | --- |
| `SpeedCheck` | 速度照査 | 平坦かつ一定の上限速度チェック。降下パターンは作動していない状態。ATSが固定の制限速度を課すデフォルトの巡航状態。 |
| `SignalP` | 信号パターン | 制限現示または停止現示の信号（閉塞・場内・出発信号）に対して課される降下パターン。 |
| `CurveP` | C信号（京急など） | 曲線または分岐器の速度制限に対して課される降下パターン。 |
| `TerminalP` | 終端パターン | 線路終端、または過走防止の側線進入を防護する降下パターン。 |
| `PApproach` | P接近 | 降下パターンに接近していることの警告。パターン自体はまだ確立していない状態。 |
| `AckPending` | 確認扱い | 確認チャイム鳴動中。非常ブレーキ動作前に運転士の確認操作を待っている状態。ATS-S等で一般的。 |
| `BApplication` | 常用ブレーキ動作 | システムによる常用ブレーキの動作（非常ブレーキではない）。 |
| `EbApplication` | 非常ブレーキ動作 | システムによる非常ブレーキの動作。 |
| `StopP` | 停車パターン・停通防止 | 駅でのオーバーラン（過走）や信号の誤通過を防止する降下パターン。 |
| `NotchCut` | ノッチカット | システムによって力行（牽引力）がカットオフされている状態。 |
| `BIsolated` | 保安装置開放 | 運転士によって保安装置が開放・遮断された状態。 |
| `Failure` | 故障 | 保安装置から報告された障害またはエラー状態。 |
| `ModeSelect` | ATS/ATC切替 | 保安システムの切り替え通知（ATS／ATC切替、路線ルールセットの切替、または車両基地・試験モードの起動）。 |
| `Other` | - | 上記に該当しない、未分類またはシミュレーター固有の状態。 |

### 5.9 `signals`

```jsonc
{
  "list": [
    {
      "name": "三田場内", // string | null：TCはネイティブ値／BVE：合成した "SecXXXm"
      "type": "Home", // 'Block' (閉塞) | 'Distant' (遠方) | 'CallOn' (誘導) | 'Shunt' (入換) | 'Home' (場内) | 'Departure' (出発) | null
      "phase": 3, // number | null：語彙に基づくint（下記）。ここでは3 = 注意(Y)
      "distance": 412, // メートル
      "transponders": [
        {
          "category": "Pattern", // 下記の語彙を参照。null = 未分類
          "code": 1003, // number | null：シミュレーターネイティブの地上子タイプコード（BVEのBeacon.Type）。取得不能時はnull
          "speedLimit": 65, // number | null：この地上子が課す制限速度（km/h、該当する場合）
          "distance": 412, // メートル。負値 = すでに通過済み
        },
      ],
    },
  ],
}
```

`list` は**最寄り順**（`distance` の昇順）にソートされます。したがって `list[0]` は列車前方で最も近い信号機を表します。

**デフォルトの地上子（トランスポンダ）カテゴリー語彙：**

| カテゴリー | 意味 |
| --- | --- |
| `Pattern` | パターン発生地上子（ATS-P／ATS-Pnパターン。制限速度は前方の信号現示から導出）。 |
| `Signal` | 信号地上子（ATS-S、SN、SW等。停止/注意/進行の現示情報を運転台へ伝送）。 |
| `TASC` | TASC／定位置停止マーカー（ホーム位置合わせのためにTASCが読み取る）。 |
| `Other` | 認識されているが、特定のカテゴリーに当てはまらない地上子。 |
| `null` | 未分類／不明。 |

`category` は、HMIのUIが描画時に条件分岐（switch-case）する対象です。`code` はシミュレーターネイティブの整数値（例：BVEの `Beacon.Type`）であり、高度なコンシューマーが完全一致によるルックアップを行えるようそのまま保持されます。シミュレーターや路線の作者が地上子の意味を定義している場合、`SimulatorProfile.vocabularies.transponders` が `code` を人間可読な文字列へマッピングします。分類できないアダプターは、推測で割り当てるのではなく `category: null` を出力しなければなりません（MUST）。推論処理は、路線固有の知識を持つコンシューマー側に委ねられます。

**デフォルトの信号現示語彙：**

| インデックス | コード | 日本語 | 意味 |
| --- | --- | --- | --- |
| 0 | : | : | 無効／故障／信号情報なし（明確な意図を持った「停止」指示であるR現示とは区別されます） |
| 1 | R | 停止 | 停止現示 |
| 2 | YY | 警戒 | 警戒現示（約25 km/h制限） |
| 3 | Y | 注意 | 注意現示（約45 km/h制限） |
| 4 | YG | 減速 | 減速現示（約65 km/h制限） |
| 5 | YGF | 抑速/YG点滅 | YG点滅／抑速現示（京急・京成等、約75〜105 km/h制限） |
| 6 | G | 進行 | 進行現示（線路最高速度） |
| 7 | GG | 高速進行 | 高速進行現示（北越急行、新幹線等） |
| 8+ | (sim/vehicle-specific) | | `SimulatorProfile.vocabularies.signalPhase` に準拠 |

インデックス設計の根拠：`0` は「無効／不明／信号情報なし」のために予約されており、これによりコンシューマーは「機能していない信号」と、明確な指示である「R（停止）現示」を区別できます。進行が許容される `1..7` の範囲内では、インデックスが大きくなるにつれて許容度（制限速度）が高くなります。各上位の数値は前の数値と同等以上の許容度を持ち、YGF（75〜105 km/h）は私鉄での運用に合わせてYG（65 km/h）とG（線路最高速度）の間に正しく配置されています。

BVEアダプターは、出力時に `Section.CurrentSignalIndex` へ `+1` を加算しなければなりません（MUST。BVEネイティブの `0=R` がRudolfの `1=R` に変換されます）。15路線のコーパス調査によりBVEネイティブのインデックス 0〜4 が検証されており、これらはRudolfへの変換後 1〜5 に対応します。デフォルトと異なる意味を使用する路線（例：BVEネイティブの `4` をGではなくYGFの意味で出力する路線）は、`SimulatorProfile.vocabularies.signalPhase` によって上書きします。

**デフォルトの信号現示制限速度テーブル：**

| 現示インデックス | デフォルト km/h | 備考 |
| ---: | ---: | --- |
| 0 | `-1` | 無効／不明。表示する制限値なし |
| 1 | `0` | R（停止） |
| 2 | `25` | YY（約25 km/h） |
| 3 | `45` | Y（約45 km/h） |
| 4 | `65` | YG（約65 km/h） |
| 5 | `90` | YGF（75〜105 km/hの中央値） |
| 6 | `-1` | G（固有の制限なし。線路最高速度） |
| 7 | `-1` | GG（固有の制限なし。高速進行） |
| 8+ | （デフォルトなし） | プロデューサーが `vocabularies.signalPhaseSpeed` を通じて公開 |

**`vocabularies.signalPhaseSpeed` の値の規約：**

- `n ≥ 0` — その現示が課す制限速度（km/h）。
- `-1` — 無制限（現示固有の制限なし。線路最高速度または路線定義の上限速度）。
- `null` — 不明（現示は存在するが、速度値が得られない）。

コンシューマーは `vocab?.signalPhaseSpeed?.[String(phase)] ?? defaults[phase]` によって実効的な現示制限速度を求めます。`?? defaults[phase]` のフォールバックは、明示的な `null` 値ではなく*欠落キー*に対してのみ適用されます。

### 5.10 `speedLimits`

```jsonc
{
  "current": 90, // km/h; -1 = 制限なし（無制限区間）
  "currentType": "SpeedLimit", // 'Signal' | 'SpeedLimit' | 'Restriction' | null
  "next": [
    // Array<{ limit, distance, type }> | null —— 前方の速度制限変化（最寄り順）。判明していなければnull。
    {
      "limit": 65, // km/h; -1 = 制限なし（無制限区間）
      "distance": 412,
      "type": "Signal", // 'Signal' | 'SpeedLimit' | 'Restriction' | null
    },
    // ...プロデューサーが把握していれば、さらに先の変化も順次格納
  ],
}
```

`type`／`currentType` の語彙：

- `'SpeedLimit'`：路線の基本制限速度（その地点における恒久的な土木構造上の制限速度）
- `'Signal'`：前方の信号現示によって課される制限（例：前方Y信号から派生したATS-Pパターン）
- `'Restriction'`：一時的または運転上の制限（曲線制限、気象による徐行命令、工事区間、駅進入制限、特別イベント徐行など）
- `null`：種別が不明または未分類（シミュレーターは制限速度値を持つが、その由来・理由が不明な場合）

**`next` の順序と完全性：** `next` は前方の速度制限変化の配列であり、**最寄り順**（`distance` の昇順）に並びます。したがって `next[0]` は前方で最も近い速度制限変化を表します。前方の変化がシミュレーター側で判明していないときは `null` となり、空配列にはなりません。直近の1件のみを把握するプロデューサーは要素数1の配列を出力し、前方の全系列を把握するプロデューサーは今後のすべての変化を出力します。プロデューサーがどちらの動作を行うかは `SimulatorProfile.capabilities['speedLimits.next']` において `NextItemArrayType` の値（§4.3.1）として宣言されます：`Single` = 直近の1件のみ、`MultiDynamic`／`MultiStatic` = 前方の全系列、`None` または省略 = 非対応。

### 5.11 `cars`

車両ごとの**動的（DYNAMIC）**状態です。車両ごとの静的データ（形式モデル、モーター/運転台/パンタグラフの有無、運転台の向き、パンタグラフタイプ、パンタグラフの向き、車両長）は `SimulatorProfile.vehicle.cars` に格納され、毎フレーム重複して送信されることはありません。

```jsonc
{
  "list": [
    {
      "carNo": 1,
      "bcPressure": 307.4, // kPa | null：TCは車両ごとにネイティブ値／BVEは[0]両目の値を全体にブロードキャスト
      "amperage": 124, // A | null：TCは車両ごとにネイティブ値／BVEは[0]両目の値を全体にブロードキャスト
      "occupancyRate": null, // 乗車率（100%を超える場合あり）| null：TCはネイティブ値／BVEはnull
    },
    // ...
  ],
}
```

車両ごとの物理演算データの正確性は `SimulatorProfile.capabilities['physics.perCar']` で宣言されます：`'true'`｜`'broadcast'`｜`'unavailable'`。

### 5.12 `switches`

```jsonc
{
  "hornAir": false, // 空気笛
  "hornElectric": false, // 電気笛
  "buzzerDriver": false, // 運転士発信の合図ブザー（車掌宛て）
  "buzzerConductor": false, // 車掌発信の合図ブザー（運転士宛て）
  "headlights": false, // 前照灯点灯状態（ロー/ハイビームの識別には `highBeam` を使用）
  "highBeam": false, // 前照灯ハイビーム
  "wiper": null, // 'Off' | 'Intermittent' (間欠) | 'Low' | 'High' | null
}
```

### 5.13 `gameState`

シミュレーター／ゲーム自体のメタ状態であり、列車の状態ではありません。

```jsonc
{
  "screen": "MainGame", // 'MainGame' | 'Pause' | 'Loading' | 'Menu' | 'Result' | 'Title' | 'NotRunning' | 'Other'
  "crewRole": "Driver", // 'Driver' (運転士) | 'Conductor' (車掌) | 'Both' | 'Others' | null
  "driveMode": "Scored", // 'Scored' (評価あり運転) | 'Unscored' (評価なし/フリー運転) | 'Other' | null
  "isOneman": false, // ワンマン運転かどうか。TCはネイティブ値／BVEはタイトル解析またはデフォルトfalse
}
```

### 5.14 拡張（Extensions）

拡張データは `extensions.<namespace>:<concern>` の下に配置されます。名前空間（namespace）にはシミュレーターID（`bve`、`traincrew`）またはベンダーIDが使用され、`concern` にはブロックに含まれる内容を指定します。

規約：

- 各拡張は、独自の `v`（セマンティックバージョン）を持つ型付きオブジェクトです。
- コンシューマーは、未知の拡張を無視して構いません（MAY）。
- プロデューサーは、コアセクションに収まる内容に対して拡張機能を使うべきではありません（SHOULD NOT）。

定義例（アダプター作成者によって定義されるものであり、Rudolfコア仕様の一部ではありません）：

```jsonc
"bve:beaconRing": {
  "list": [
    { "type": 1003, "passedAt": 12300.1, "data": 5, "optional": 0 }
  ]
}

"bve:atsPanelArray": {
  "raw": [0, 0, 1, 0, 1, /* ... ほかに1019個の値 ... */]
}

"traincrew:ato": {
  "active": true,
  "notch": -3,
  "targetSpeed": 65
}
```

## 6. InputCommand

コンシューマーからシミュレーター（consumer → sim）への通信。1つの `InputCommand` ドキュメントにつき1つのコマンドが格納されます（バッチ処理は、将来的に必要に応じて明示的な拡張として定義されます）。コマンド送信側は、`SimulatorProfile.capabilities` でサポートが宣言されているコマンドのみを送信すべきです（SHOULD）。

```jsonc
{
  "schemaVersion": "1.0",
  "kind": "InputCommand",
  "scenarioId": "51a35aec-d930-455f-a8fa-58f686f87254",
  "sentAt": "2026-06-25T14:23:17.350Z",
  "sequenceNumber": 1042, // long：コンシューマーごとに単調増加する値（順序制御および冪等性のため）
  "command": {
    "kind": "SetNotch",
    "value": -2,
  },
}
```

### 6.1 コマンドの種類

すべてのコマンドは `command.kind` によって判別されます。一覧：

| 種別 | ペイロード | 意味・動作 |
| --- | --- | --- |
| `SetNotch` | `{ value: int, relative?: bool }` | 統合ノッチ（総括ノッチ）。`relative`（デフォルト `false`）= 絶対指定：valueは統合ノッチ値（0=N、+n=Pn、-1=抑速、-2…=B1…）。`relative: true` = 符号付きステップ差分指定。いずれの場合も、`value <= -100`（センチネル定数 `EB = -100`）は非常ブレーキを表し、車両形式に依存せず、従来のハードコードされた -8 に取って代わります。 |
| `SetPowerNotch` | `{ value: int }` | 力行専用ノッチ。正の整数。 |
| `SetBrakeNotch` | `{ value: int }` | ブレーキ専用ノッチ。正の整数。 |
| `SetBrakeSAP` | `{ kPa: double }` | 電磁直通ブレーキ（SAP）の目標圧力値。0〜400 = 常用ブレーキ、410 = 非常ブレーキ。 |
| `SetReverser` | `{ value: int }` | レバーサー（逆転器）位置。`-1` = 後進、`0` = 中立、`1` = 前進。この範囲外の値は拒否されなければなりません（MUST）。 |
| `SetButton` | `{ action: string, state: bool }` | 汎用ボタン操作。`action` は `VehicleAction`（§6.2）または `GameAction`（§6.3）の名前、あるいはカスタムアクション文字列です。カスタム／仕様外のアクションは検証なしのパススルーとして扱われ、`capabilities['input.button.<action>']` によって有効化されます。 |
| `SetWiper` | `{ state: 'Off'\|'Intermittent'\|'Low'\|'High' }` | ワイパー位置。 |
| `SetAtoNotch` | `{ value: int }` | ATO推奨ノッチ値。TRAIN CREWの仕様：notch > 0 のときは手動ノッチがNの場合のみ適用。notch < 0 のときは手動とATOのうち大きい方のブレーキ力を適用。 |
| `SetDeadman` | `{ method: 'Hand'\|'Foot'\|'EB', holding: bool }` | 方式ごとのデッドマンスイッチ操作状態。 |

必須として記述されたフィールドは必ず設定しなければなりません（MUST）。省略可能なフィールド（OPTIONAL）は、コマンドごとに文書化されたデフォルト動作が適用されます。

> **`SetNotch` の非常ブレーキセンチネル**：予約された定数 `EB = -100`（`value <= -100` のすべて）は、`relative` の指定にかかわらず非常ブレーキを要求します。生のリテラル値よりもこの定数の使用を推奨します。これは車両形式に依存せず、従来のハードコードされた `-8` に取って代わるものです。
>
> **カスタム `SetButton` アクション**：`VehicleAction`（§6.2）および `GameAction`（§6.3）は、そのメンバー名がそのまま `action` 文字列にシリアライズされる仕様上の語彙です。この語彙に含まれないアクションは、同一の文字列フィールドを介してカスタムアクションとして扱われ、シミュレーター側は `capabilities['input.button.<action>']` で対応を宣言します。

### 6.2 VehicleAction 列挙型

`SetButton` で使用する、物理的な運転台・車両機器の操作です。語彙はTRAIN CREW SDKをベースに、より整理された命名体系へ改められています。各エントリには定義されたセマンティクス（意味）がありますが、すべてのシミュレーターが全項目に対応しているとは限りません。詳細は `SimulatorProfile.capabilities['input.button.<action>']` を確認してください。ノッチ操作はボタンアクションではなくなり、`SetNotch`（§6.1）を使用します。旧 `InputAction` からの改名：`Broadcast` → `InCarBroadcast`, `LightLow` → `HeadLightLow`。

- `EBReset`：EB／デッドマン警報のリセット（EB復帰）
- `GradientStart`：勾配起動スイッチの作動（転動防止）
- `SafetyBrake`：保安ブレーキスイッチ（保安ブレーキ）
- `SnowBrake`：耐雪ブレーキスイッチ（耐雪ブレーキ）
- `HornAir`：空気笛の吹鳴（空気笛）
- `HornElectric`：電気笛の吹鳴（電気笛）
- `Buzzer`：連絡ブザーの鳴動（合図ブザー）
- `BoardingPrompt`：乗降促進ブザー／放送の作動（乗降促進）
- `InCarBroadcast`：車内放送／PAの再生（車内放送） — 旧 `Broadcast`
- `DoorOpenLeft`：左側客用ドアを開く（左ドア開）
- `DoorCloseLeft`：左側客用ドアを閉じる（左ドア閉）
- `DoorOpenRight`：右側客用ドアを開く（右ドア開）
- `DoorCloseRight`：右側客用ドアを閉じる（右ドア閉）
- `DoorReopen`：閉扉中断後の再開閉スイッチ（再開閉SW）
- `DoorKey`：ドアスイッチ鍵の操作（ドアスイッチ鍵）
- `PartialDoor`：3/4ドア一部締切スイッチ（3/4閉スイッチ）
- `DoorCut`：ドアカットスイッチ（ドアカットSW）
- `HeadLightLow`：前照灯の減光／ロービーム（前灯減光） — 旧 `LightLow`
- `HeadLight`：前照灯スイッチ（前照灯SW）
- `CabinLight`：客室灯スイッチ（客室灯SW）
- `CrewRoomLight`：乗務員室灯スイッチ（乗務員室灯SW）
- `InstrumentLight`：計器灯スイッチ（計器灯SW）

### 6.3 GameAction 列挙型

`SetButton` で使用する、カメラ／視点／UI／シミュレーターメタ操作のアクションです。これらは省略可能であり（OPTIONAL）、コンシューマーはこれらがサポートされていることに依存すべきではありません（SHOULD NOT）。`SimulatorProfile.capabilities['input.button.<action>']` を確認してください。

**カメラ／視点操作：**

- `ExteriorView`：外部視点への切り替え（外部視点切替）
- `DriverAlternateView`：運転士の別視点切り替え
- `ConductorAlternateView`：車掌の後方確認視点（後方確認）
- `LeftWindowView`：左側窓からの眺望視点
- `RightWindowView`：右側窓からの眺望視点

**シミュレーターUI／メタ操作：**

- `TogglePauseMenu`：ポーズメニューの表示切替
- `ToggleDiagramDisplay`：スタフ／時刻表の表示切替（スタフ表示）
- `ToggleGUI`：ゲーム内UIの表示切替（画面表示）
- `ToggleCrewDoor`：乗務員室ドアの開閉切替
- `ToggleCrewWindow`：乗務員室窓の開閉切替

## 7. 通信トランスポート

Rudolfはドキュメントのデータ構造を定義しますが、**トランスポート方式には依存しません**。

推奨されるトランスポートバインディング：

- HTTP
- WebSocket／Socket.IO
- 共有メモリ（Windows）

## 8. ペイロード例

### 8.1 SimulatorProfile（TRAIN CREW）

```json
{
  "schemaVersion": "1.0",
  "kind": "SimulatorProfile",
  "scenarioId": "51a35aec-d930-455f-a8fa-58f686f87254",
  "sentAt": "2026-07-02T20:18:18.3444612+00:00",
  "sequence": 1,
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
    "physics.perCar": "True",
    "ats.richState": true,
    "speedLimits.next": "Single",
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

### 8.2 OutputDataFrame（TRAIN CREW）

```json
{
  "schemaVersion": "1.0",
  "kind": "OutputDataFrame",
  "scenarioId": "51a35aec-d930-455f-a8fa-58f686f87254",
  "sentAt": "2026-07-02T20:19:26.6283871+00:00",
  "time": {
    "sim": "07:51:50",
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
    "curveRadius": null,
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
    "values": [1, 1, 0, 0, /* ... 合計512個の値 */]
  },
  "ats": {
    "class": "普通",
    "speed": 110,
    "state": null,
    "richState": []
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
  "speedLimits": {
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

### 8.3 InputCommandの例

```jsonc
// ブレーキノッチをB3へ
{ "schemaVersion": "1.0", "kind": "InputCommand", "scenarioId": "...", "sentAt": "...", "sequenceNumber": 1042,
  "command": { "kind": "SetBrakeNotch", "value": 3 } }

// 空笛を押す
{ "schemaVersion": "1.0", "kind": "InputCommand", "scenarioId": "...", "sentAt": "...", "sequenceNumber": 1043,
  "command": { "kind": "SetButton", "action": "HornAir", "state": true } }

// 空笛を離す
{ "schemaVersion": "1.0", "kind": "InputCommand", "scenarioId": "...", "sentAt": "...", "sequenceNumber": 1044,
  "command": { "kind": "SetButton", "action": "HornAir", "state": false } }
```
