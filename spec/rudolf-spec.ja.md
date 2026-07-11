# Rudolf仕様書

[English](./rudolf-spec.md) | **日本語**

## 1. 用語

| 用語                       | 意味                                                                                                                             |
| -------------------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| Sim（シム）                | Rudolfドキュメントを出力する列車シミュレーター（BVE、TRAIN CREWなど）。                                                          |
| Adapter（アダプター）      | シミュレーターのネイティブAPIを読み取り、ワイヤー上のRudolf形式へ変換するコード。                                                |
| Consumer（コンシューマー） | Rudolfドキュメントを読み取るもの全般：HMIディスプレイ、ドライブレコーダー、Webダッシュボード、物理デバイスのコントローラーなど。 |
| Producer（プロデューサー） | Rudolfドキュメントを出力するもの全般：通常はシミュレーターの内部または隣で動作するアダプター。                                   |
| Section（セクション）      | `OutputDataFrame`のトップレベルキー（例：`physics`、`signals`）。                                                                |
| Extension（拡張）          | `extensions:`配下に置かれる、名前空間付きのシミュレーター固有・ベンダー固有ブロック（例：`bve:beaconRing`）。                    |
| Scenario（シナリオ）       | シナリオの読み込みから終了までの、シミュレーターの1回のプレイセッション。                                                        |
| HMI                        | ヒューマンマシンインターフェース。すなわち列車情報管理装置（TIMS／INTEROS）。                                                    |

## 2. ドキュメントの種類

Rudolfは3種類のドキュメントを定義します。いずれもJSONで、UTF-8エンコード、camelCaseです。

| ドキュメント       | 方向           | 頻度                                        | 目的                                     |
| ------------------ | -------------- | ------------------------------------------- | ---------------------------------------- |
| `SimulatorProfile` | sim → consumer | シナリオ読み込み時に1回（および車両変更時） | 静的メタデータ、capabilities、語彙マップ |
| `OutputDataFrame`  | sim → consumer | フレームごと（通常4 Hz／250 ms）            | 列車＋ゲーム状態のライブスナップショット |
| `InputCommand`     | consumer → sim | 入力イベントごと                            | デバイスコマンド（ノッチ、ボタンなど）   |

すべてのドキュメントは次のフィールドを持ちます。

- `schemaVersion: string`：Rudolf仕様のバージョン。現行バージョン：`"1.0"`。
- `kind: 'SimulatorProfile' | 'OutputDataFrame' | 'InputCommand'`：ドキュメント種別の判別子。
- `scenarioId: string`：1つのプレイセッションに属するすべてのドキュメントを結び付ける不透明な識別子。同一の`scenarioId`が、そのSimulatorProfile、そのシナリオ内のすべてのOutputDataFrame、およびそれを対象とするすべてのInputCommandに現れます。
- `sentAt: string`：プロデューサー側でのISO 8601タイムスタンプ。

## 3. アーキテクチャ

### 3.1 エンベロープの規約

#### 命名規約

ワイヤー上ではcamelCaseを使用します。C# のプロデューサーは`CamelCasePropertyNamesContractResolver`によってPascalCaseから変換します。TypeScript／JavaScriptのコンシューマーはcamelCaseをそのまま読み取ります。

#### 文字列エンコーディング

すべての文字列値はリテラルなUTF-8として出力されます —— **`\uXXXX`のエスケープシーケンスは使いません**。日本語テキスト（駅名、路線名、車両名）はワイヤー上にそのまま現れなければなりません（例：`\u7ACB\u4F1A\u5DDD`のようにエスケープするのではなく、`"立会川"`）。プロデューサーは、それに合わせてシリアライザーのエンコーダーを設定します（例：.NETの`JavaScriptEncoder.UnsafeRelaxedJsonEscaping`）。これらのドキュメントが生のHTMLに埋め込まれることはないため、出力をHTMLエスケープする必要はありません。いずれにせよコンシューマーは両方の形式を受け入れなければなりません（`\u`エスケープされたJSONも、デコードすれば同じ文字列になります）。

#### 単位

- 速度：**km/h**
- 圧力：**kPa**
- 距離／位置：**メートル**
- 勾配：**‰**（パーミル）
- 電流：**A**（アンペア）
- 時刻：ISO 8601文字列。実日付を持たないシミュレーターでは`Kind=Unspecified`を許容します。

#### null許容フィールド

`null`が設定されたフィールドは、「シミュレーターが今この値を本当に持っていない」ことを意味します。

JSONに存在しないフィールドは、「シミュレーターがこのフィールドをそもそもサポートしていない」ことを意味します（これはSimulatorProfile.capabilitiesでも示されます）。通常は生成するものの現時点で値がないフィールドについては、プロデューサーは省略よりも`null`を優先すべきです。

#### バージョニング

すべてのドキュメントは、エンベロープレベルに単一の`schemaVersion`を持ちます。いずれかのセクションへの破壊的変更があれば`schemaVersion`を更新します。コンシューマーは、将来のマイナーバージョンで追加される未知のフィールドを許容しなければなりません（知っているものを読み、知らないものは無視する）。

### 3.2 ドキュメント構造

```
SimulatorProfile = { schemaVersion, kind, scenarioId, sentAt, sim, scenario, vehicle, capabilities, vocabularies }

OutputDataFrame = { schemaVersion, kind, scenarioId, sentAt,
                time, diagram, stations, physics, controllers, doors,
                lamps, ats, signals, speedLimit, cars, switches, gameState,
                extensions? }

InputCommand = { schemaVersion, kind, scenarioId, sentAt, sequenceNumber, command }
```

### 3.3 拡張性

- **拡張ブロック：** `extensions.<sim>:<concern>`という名前空間を使います。例：`bve:beaconRing`、`bve:atsPanelArray`。サードパーティは独自のブロックを自由に定義できます。
- **語彙（Vocabularies）：** デフォルトの語彙（信号現示、ランプキー、地上子タイプの意味）は本仕様で公開されています。SimulatorProfile.vocabulariesによってシナリオごとに上書きできます。

## 4. SimulatorProfile

シナリオ読み込み時に1回送信されます。車両変更時に再送されます。`scenarioId`によってキャッシュ可能です。

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
    "transponders": null
  }
}
```

### 4.1 `vehicle.capabilities`

車両の静的な制御機器情報です。トップレベルの`capabilities`マップ（アダプターがどの`OutputDataFrame`フィールドを実際に生成するかを宣言するもの）とは別物です。すべてのフィールドはnull許容で、`null`は「シミュレーターが現時点で値を持たない」ことを意味します。

- `masconType`：マスコンのハンドル方式。`'OneHandle' | 'TwoHandle' | null`（MasconType）。
- `masconBrakeType`：ブレーキハンドルの挙動。`'Notched' | 'LapCapable' | 'Continuous' | null`（MasconBrakeType）。`LapCapable`はラップ付きの連続ブレーキ（＝連続を含意）、`Continuous`はラップ位置を持たない非段階（直通）ハンドルです。
- `powerNotches`：力行ノッチ数（例：P1〜P5なら5）。不明な場合は`null`。
- `brakeNotches`：常用ブレーキノッチ数（例：B1〜B7なら7）。不明な場合は`null`。
- `ebNotch`：SetNotchエンコーディングにおいてEBを表す符号付きノッチ値（例：`-8`）。不明な場合は`null`。
- `holdingBrakeNotches`：抑速ブレーキのノッチ数。備えていない場合は`0`、不明な場合は`null`。
- `cpStartPressure`／`cpStopPressure`：空気圧縮機の始動／停止圧力（kPa）。不明な場合は`null`。

### 4.2 `vehicle.name`, `vehicle.model` & `vehicle.operator`

- `name`：車両モデルの表示名（例：`"225系0番台"`）。系や番台の漢字が正しいことを確認してください。編成に複数のモデルが含まれる場合は、`+`で区切ってください（例：`"E231系1000番台+E233系3000番台"`）。
- `model`：車両モデルの識別子（例：`"225-0"`）。相互運用性を最大化するため、`series-subseries`形式にしてください。かなはすべてTitleCaseでローマ字化します。編成に複数のモデルが含まれる場合は、`+`で区切ってください（例：`"E231-1000+E233-3000"`）。
- `operator`：運営会社（例：`"EastJapanRailwayCompany"`、`"TokyuCorporation"`）。互換性を最大化するため、（グループ名ではなく）正式な事業者名を日本語版Wikipediaで確認し、TitleCaseにしてください。

## 5. OutputDataFrame

フレームごとに送信されます（通常4 Hz程度。シミュレーターはこれより速くも遅くも出力できます）。コアとなる各セクションのキーは、構造上つねに存在します（空の場合でも）。セクション内のフィールドはnullになることがあります。

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
  "sim": "10:34:22", // dateKnown=falseのときは "HH:MM:SS" 形式の時刻のみ／trueのときはISO日時
  "dateKnown": false, // シミュレーターが実日付を提供する場合はtrue
  "elapsed": 412.5, // シナリオ開始からの秒数。単調増加
  "tick": 1650, // フレームカウンター。出力ごとに増加
}
```

### 5.2 `diagram`

寛容な設計です：アダプターは、シミュレーターがネイティブに知っている情報だけを埋めます。ヒューリスティックな導出は規定しません。コンシューマーは、必要であればローカルで導出値を計算してもかまいません。

```jsonc
{
  "trainNumber": "1234A", // string | null：TCのdiaName／BVE：ScenarioInfo.Titleから解析
  "boundFor": "館浜", // string | null：TCはネイティブ／BVE：可能ならタイトルから解析
  "serviceType": "普通", // string | null：TCはネイティブ／BVE：タイトルのキーワード一致
  "direction": null, // 'Upbound' | 'Downbound' | null：LineDirection。Upbound=上り、Downbound=下り
  "runNumber": null, // string | null：シミュレーターネイティブのみ。導出はしない
}
```

コンシューマーは、必要に応じて「終点までの残り距離」を`stations.list[last].fromStartDistance - physics.fromStartDistance`として計算します。

### 5.3 `stations`

```jsonc
{
  "list": [
    {
      "index": 0,
      "name": "起点",
      "fromStartDistance": 0, // シナリオ開始地点からのメートル数。つねに存在
      "absoluteDistance": 35403.2, // meters | null：絶対キロ程。TCはつねにnull
      "doorSide": 1, // int：-1=左、0=なし／不明、1=右、2=両側
      "stopType": "PassengerStop", // 'PassengerStop' | 'OperationStop' | 'Passing' | null
      "arrival": null,
      "departure": "10:00:00",
      "stopPositionName": "下り1番線", // string | null
      "isTimeTaken": true, // bool | null：採時駅かどうか。シミュレーターが扱わない場合はnull
      "stopPositions": [3, 4, 6], // number[] | null：現在の方向／番線における停止目標の両数候補。不明時はnull
    },
    // ... 駅ごとに
  ],
  "currentIndex": null, // number | null：列車が現在いる駅
  "nextIndex": 5, // number | null：前方の次駅
}
```

`name`は駅の表示名の**み**です —— 駅コードや駅ナンバリングは含みません（例：`"品川"`。`"KK01品川"`、`"品川(JK20)"`、`"KK01"`などは不可）。他のすべての文字列と同様、`\u`エスケープシーケンスを使わないリテラルなUTF-8として出力されます（§3.1の「文字列エンコーディング」を参照）。

コンシューマーは、参照（ルックアップ）によって完全な駅レコードと次駅までのライブ距離を導出します。

```js
const next =
  stations.nextIndex != null ? stations.list[stations.nextIndex] : null;
const distanceToNext =
  next != null ? next.fromStartDistance - physics.fromStartDistance : null;
```

### 5.4 `physics`

```jsonc
{
  "speed": 78.4, // km/h。列車単位。つねに存在
  "fromStartDistance": 12345.6, // シナリオ開始地点からの走行メートル数。つねに存在
  "absoluteDistance": 47823.6, // meters | null：路線上の絶対キロ程
  "gradient": null, // ‰ | null：BVE 2.0.8は公開していない
  "mrPressure": 740.0, // kPa。列車単位。つねに存在
}
```

- `fromStartDistance`はつねに存在します：シナリオ開始からの走行メートル数です。通常運転中は単調増加します（列車が後退するときのみ減少）。
- `absoluteDistance`は、公式に測量されたキロ程です。路線をまたいだ対応付け、ATS地上子の参照、緯度経度へのマッピングに役立ちます。シミュレーターがシナリオ相対の距離しか知らない場合はnullになります。

車両ごとのBC圧力（ブレーキシリンダー圧力）と電流値は`cars`にあります。

### 5.5 `controllers`

```jsonc
{
  "powerNotch": 2, // TCのPnotch／BVEのHandles.PowerNotch
  "brakeNotch": 0, // TCのBnotch／BVEのHandles.BrakeNotch
  "reverser": 1, // int：-1=後進、0=中立、1=前進
  "ato": null, // { active: bool, notch?: number } | null
  "tasc": null, // { active: bool, notch?: number, inching: bool } | null
  "deadman": null, // 'Hand' | 'Foot' | 'EB' | null：現在作動しているチャンネル
}
```

- `ato`は、列車で自動列車運転装置（ATO）が有効なとき非nullになります。`notch`（省略可能）は、ATOが指示しているノッチです。
- `tasc`（定位置停止装置）は、次の停車に向けてTASCが有効なとき非nullになります。`active`は、TASCがブレーキ指示を制御しているときtrueになります。`inching`は、TASCが最終の低速位置合わせ段階（ホームの停止目標に合わせる小刻みな微調整）にあるときtrueになります。`notch`（省略可能）は、TASCが指示しているノッチです。

注意：`reverser`は`-1 = 後進、0 = 中立、1 = 前進`という規約のintです。TCはこの範囲外の値をネイティブに出力します（例：`-2`はブレーキ優先セレクターの切り替えであり、実車のレバーサー機能ではありません）。Rudolfのアダプターは、こうした非標準の値をそのまま通すのではなく、最も近い自然なレバーサー位置へクランプしなければなりません（または省略／直前の値を保持）。コンシューマーは、レバーサーがつねに`{-1, 0, 1}`のいずれかであると仮定してよいものとします。

### 5.6 `doors`

```jsonc
{
  "allClosed": true,
  "perCar": [
    { "carNo": 1, "sideOpened": 0 },
    { "carNo": 2, "sideOpened": 0 },
    { "carNo": 3, "sideOpened": 1 }, // この車両は右側が開いている
    { "carNo": 4, "sideOpened": 3 }, // 開いているがどちら側か不明（例：TRAIN CREW）
    // ...
  ],
}
```

`sideOpened`は`int | null`です。規約は`stations.list[].doorSide`に準じますが、「開いているがどちら側か不明」を表す正の値（`3`）が加わり、`null`は仕様全体で共通の「値なし」の意味（§3.1）に限定されます。

- `-1` = 左側が開
- `0` = 閉（この車両のすべてのドアが閉：既知の閉状態）
- `1` = 右側が開
- `2` = 両側が開
- `3` = 開いているがどちら側か不明（ドアが開いているのは分かるが左右を区別できない）
- `null` = この車両のドア値が存在しない（仕様§3.1）

補足：

- `allClosed`（列車単位のbool）は、第一級のフィールドとして維持されます：両シミュレーターがネイティブに提供し（`TC TrainState.AllClose`、`BVE DoorSet.AreAllClosed`）、HMIにとって「発車して安全か？」を判断する要となる指標です。
- TCは車両ごとに1つのboolしか持ちません（左右の区別なし）：TCアダプターは、閉のとき`0`、開のとき`3`（開いているがどちら側か不明）を出力します（左右をネイティブに判定できないため）。BVEアダプターは、実際の左右データ（`-1`／`1`／`2`）を出力します。

### 5.7 `lamps`

```jsonc
{
  "values": {
    "doorClose": 1,
    "atsReady": 1,
    "atsBrakeApply": 0,
    "regenerative": 1,
    "pilot": 1,
    // シミュレーター／車両固有のキーも自由に使用可
  },
}
```

**状態の規約：**

- `0` = 消灯
- `1` = 点灯
- `2`以上 = 車両固有の別状態（点滅、減光、多色など）。UIはこれらを解釈してもしなくてもよい。0／1しか知らない基本的なHMIは、0以外をすべて真として扱うべきです。

**デフォルト語彙**（コンシューマーが知っておくべきもの）：`doorClose, atsReady, atsBrakeApply, atsOpen, regenerative, ebTimer, emergencyBrake, overload, pilot, ato`。

**BVE固有：** `AtsPanelArray[1024]`（車両作者の慣習）は、出力前に`SimulatorProfile.vocabularies.lamps.bveIndexToKey`によって名前付きキーへマッピングされます。生の配列は、高度なコンシューマー（車両プラグインごとのデバッガーなど）向けに、`extensions["bve:atsPanelArray"]`にも追加で現れることがあります。

### 5.8 `ats`

```jsonc
{
  "class": "ATS-P", // string | null：TCのATS_Class／BVE：ファミリーごとのプロファイルから（v1：通常はnull）
  "speed": -1, // number | null：現在のATS速度制限。-1 = フリー（無制限）／null = 表示なし／それ以外はkm/h
  "state": "P接近", // string | null：TCのATS_State（リッチ）／BVE v1：'EB' またはnull
  "richState": null, // { code: string[], name: string[], severity: number[], type: AtsRichStateType[] } | null：並列配列。インデックスN = N番目の有効な状態
}
```

`ats.speed`の規約：`-1` = フリー（無制限／ATSが上限を課していない）、`null` = 表示ブランク（表示する値がない）、それ以外の数値 = 課されている速度上限（km/h）。これは、TCの「F」をマジックナンバー`300`に対応させるハックや、以前の`'free'`という文字列センチネルを置き換えるものです。すべての値が数値（またはnull）になったため、コンシューマーはユニオン型を扱う必要がありません。

**`richState`の構造：** 非nullのとき、`richState`は`code`、`name`、`severity`、`type`という4つの並列配列を持ちます。4つすべてにわたるインデックスNは、同時に有効なN番目のATS状態を表します。`code`はシミュレーターの生の自由形式文字列（例：`"P_APPROACH"`）、`name`は表示ラベル（例：`"P接近"`）、`severity`は`0`（情報）／`1`（警告）／`2`（重大）で、`2`を超える値はシミュレーター・車両固有のカスタム重大度に予約されています。`type`は下記の語彙による機械可読なカテゴリーです。

**`AtsRichStateType`の語彙：**

| 値              | 日本語での呼称         | 意味                                                                                                                    |
| --------------- | ---------------------- | ----------------------------------------------------------------------------------------------------------------------- |
| `SpeedCheck`    | 速度照査               | 平坦で連続的な上限速度の照査。低下パターンは作動していない。ATSが固定の速度制限を課しているときのデフォルトの走行状態。 |
| `SignalP`       | 信号パターン           | 制限現示または停止現示の信号（閉塞・場内・出発信号）に対して課される低下パターン。                                      |
| `CurveP`        | C信号（京急など）      | 曲線または分岐器の速度制限に対して課される低下パターン。                                                                |
| `TerminalP`     | 終端パターン           | 線路終端、またはオーバーラン注意の側線進入を防護する低下パターン。                                                      |
| `PApproach`     | P接近                  | 低下パターンに接近しているという警告。パターンはまだ確立していない。                                                    |
| `AckPending`    | 確認扱い               | チャイムが鳴動中。非常ブレーキ動作前の運転士の確認を待っている。ATS-Sでよく見られる。                                   |
| `BApplication`  | 常用ブレーキ動作       | システムによる常用ブレーキの動作（非常用ではない）。                                                                    |
| `EbApplication` | 非常ブレーキ動作       | システムによる非常ブレーキの動作。                                                                                      |
| `StopP`         | 停車パターン・停通防止 | 駅でのオーバーランや信号の誤通過を防止する低下パターン。                                                                |
| `NotchCut`      | ノッチカット           | システムによって強制される力行のカット。                                                                                |
| `BIsolated`     | 保安装置開放           | 運転士によって保安装置がカットアウト（開放）された状態。                                                                |
| `Failure`       | 故障                   | 保安装置が報告する障害またはエラー状態。                                                                                |
| `ModeSelect`    | ATS/ATC切替            | 保安システムの切り替え通知（ATS／ATCの切替、路線ルールセットの切替、または車庫・試験モードの起動）。                    |
| `Other`         | -                      | 上記に該当しない、分類外またはシミュレーター固有の状態。                                                                |

### 5.9 `signals`

```jsonc
{
  "list": [
    {
      "name": "三田場内", // string | null：TCはネイティブ／BVE：合成した "SecXXXm"
      "type": "Home", // 'Block' | 'Distant' | 'CallOn' | 'Shunt' | 'Home' | 'Departure' | null
      "phase": 3, // number | null：語彙付きのint（下記）。ここでは3 = Y
      "distance": 412, // メートル
      "transponders": [
        {
          "category": "Pattern", // 下記の語彙を参照。null = 未分類
          "code": 1003, // number | null：シミュレーターネイティブの地上子タイプコード（BVEのBeacon.Type）。公開されない場合はnull
          "speedLimit": 65, // number | null：この地上子が課す速度制限（km/h、該当する場合）
          "distance": 412, // メートル。負値 = すでに通過済み
        },
      ],
    },
  ],
}
```

`list`は近い順（`distance`の昇順）に並びます。したがって`list[0]`は、列車の前方で最も近い信号です。

**デフォルトの地上子カテゴリー語彙：**

| カテゴリー | 意味                                                                                |
| ---------- | ----------------------------------------------------------------------------------- |
| `Pattern`  | パターン発生の地上子（ATS-P／ATS-Pnのパターン。速度制限は前方の信号現示から導出）。 |
| `Signal`   | 信号の地上子（ATS-S、SN、SW。R／Y／Gの情報を運転台へ伝える）。                      |
| `TASC`     | TASC／停止位置マーカー（ホーム合わせのためにTASCが読み取る）。                      |
| `Other`    | 認識はされるが、特定のカテゴリーに当てはまらない。                                  |
| `null`     | 未分類／不明。                                                                      |

`category`は、HMIのUIが描画時に分岐（switch-case）する対象です。`code`はシミュレーターネイティブのint（例：BVEの`Beacon.Type`）で、高度なコンシューマーが完全一致で参照できるようそのまま保持されます。シミュレーターや路線の作者が意味を登録している場合、`SimulatorProfile.vocabularies.transponders`が`code`を人間可読な文字列へマッピングします。分類できないアダプターは、推測するのではなく`category: null`を出力しなければなりません。推論は、路線固有の知識を持つコンシューマーに委ねます。

**デフォルトの信号現示語彙：**

| インデックス | コード                 | 日本語      | 意味                                                                  |
| ------------ | ---------------------- | ----------- | --------------------------------------------------------------------- |
| 0            | :                      | :           | 無効／故障／信号情報なし（意図的な「停止」現示であるRとは区別される） |
| 1            | R                      | 停止        | 停止                                                                  |
| 2            | YY                     | 警戒        | 警戒（約25 km/h）                                                     |
| 3            | Y                      | 注意        | 注意（約45 km/h）                                                     |
| 4            | YG                     | 減速        | 減速（約65 km/h）                                                     |
| 5            | YGF                    | 抑速/YG点滅 | YG点滅／抑速（京急・京成、約75〜105 km/h）                            |
| 6            | G                      | 進行        | 進行（線路最高速度）                                                  |
| 7            | GG                     | 高速進行    | 高速進行（北越急行、新幹線）                                          |
| 8+           | (sim/vehicle-specific) |             | SimulatorProfile.vocabularies.signalPhaseに従う                       |

インデックス設計の根拠：`0`は「無効／不明／信号情報なし」のために予約されています。これにより、コンシューマーは機能していない信号と、意図的なR現示（それは指示の不在ではなく、実際の指示です）を区別できます。進行順に並んだ範囲`1..7`の中では、インデックスは許容度が高くなるほど大きくなります。各上位の数値は前の数値と同等以上に許容的であり、YGFはYG（65 km/h）とG（線路最高速度）の間に正しく位置づけられます。YGFを使用する私鉄では75〜105 km/hを許容するためです。

BVEアダプターは、出力時に`Section.CurrentSignalIndex`へ`+1`しなければなりません（BVEネイティブの`0=R`がRudolfの`1=R`になる、など）。15路線のコーパス調査によりBVEネイティブのインデックス0〜4が検証されており、これらはRudolfへの変換後1〜5になります。デフォルトと異なる意味を使う路線（例：BVEネイティブの`4`をGではなくYGFの意味で出力する路線）は、`SimulatorProfile.vocabularies.signalPhase`によって上書きします。

### 5.10 `speedLimit`

```jsonc
{
  "current": 90, // km/h
  "currentType": "SpeedLimit", // 'Signal' | 'SpeedLimit' | 'Restriction' | null
  "next": [
    // Array<{ limit, distance, type }> | null —— 今後の変化。近い順。判明していなければnull。
    {
      "limit": 65,
      "distance": 412,
      "type": "Signal", // 'Signal' | 'SpeedLimit' | 'Restriction' | null
    },
    // ...プロデューサーが把握していれば、さらに先の変化も
  ],
}
```

`type`／`currentType`の語彙：

- `'SpeedLimit'`：路線に掲示された基本の速度制限（この地点の恒久的な土木上の制限）
- `'Signal'`：前方の信号現示によって課される制限（例：前方のY現示から導出されるATS-Pパターン）
- `'Restriction'`：一時的または運転上の制限（曲線制限、天候による徐行、工事区間、駅進入制限、特別イベントの徐行）
- `null`：種別が不明または未分類（シミュレーターは制限値を持つが、その由来は持たない）

**`next`の順序と完全性：** `next`は今後の速度制限変化の配列で、近い順（`distance`の昇順）に並びます。したがって`next[0]`は、前方で最も近い変化です。今後の変化がシミュレーターに判明していないときは`null`になります —— 空配列にはなりません。直近の次の変化しか把握しないプロデューサーは要素1つの配列を出力し、前方の全系列を把握するプロデューサーは今後のすべての変化を出力します。プロデューサーがどちらを行うかは`SimulatorProfile.capabilities['speedLimit.next']`で宣言されます：`'full'`（今後のすべての変化を列挙）｜`'single'`（直近の次のみ）｜`false`／省略（非対応）。

### 5.11 `cars`

車両ごとの**動的**状態です。車両ごとの静的データ（model、hasMotor/Cab/Pantograph、cabDirection、pantographType、pantographDirection、length）は`SimulatorProfile.vehicle.cars`にあり、フレームごとには重複させません。

```jsonc
{
  "list": [
    {
      "carNo": 1,
      "bcPressure": 307.4, // kPa | null：TCは車両ごとにネイティブ／BVE：[0] からブロードキャスト
      "amperage": 124, // A | null：TCは車両ごとにネイティブ／BVE：[0] からブロードキャスト
      "occupancyRate": null, // 乗車率（100% を超えることもある）| null：TCはネイティブ／BVE：null
    },
    // ...
  ],
}
```

車両ごとの物理量が本物かどうかは`SimulatorProfile.capabilities['physics.perCar']`で宣言されます：`'true'`｜`'broadcast'`｜`'unavailable'`。

### 5.12 `switches`

```jsonc
{
  "hornAir": false,
  "hornElectric": false,
  "buzzerDriver": false, // 運転士が発信（車掌へ）
  "buzzerConductor": false, // 車掌が発信（運転士へ）
  "headlights": false, // bool：前照灯が点灯しているときtrue（ロービーム／ハイビームの区別は`highBeam`を使用）
  "highBeam": false,
  "wiper": null, // 'Off' | 'Intermittent' | 'Low' | 'High' | null
}
```

### 5.13 `gameState`

シミュレーター／ゲームのメタ状態です。列車の状態ではありません。

```jsonc
{
  "screen": "MainGame", // 'MainGame' | 'Pause' | 'Loading' | 'Menu' | 'Result' | 'Title' | 'NotRunning' | 'Other'
  "crewRole": "Driver", // 'Driver' | 'Conductor' | 'Both' | 'Others' | null
  "driveMode": "Scored", // 'Scored' | 'Unscored' | 'Other' | null
  "isOneman": false, // TCはネイティブ／BVE：タイトルから解析、なければfalseをデフォルト
}
```

### 5.14 拡張（Extensions）

拡張は`extensions.<namespace>:<concern>`の下に置かれます。namespaceはシミュレーターID（`bve`、`traincrew`）またはベンダーIDです。concernは、そのブロックが何を含むかを表します。

規約：

- 各拡張は、独自の`v`（セマンティックバージョン）を持つ型付きオブジェクトです
- コンシューマーは、未知の拡張を無視してもよい
- プロデューサーは、コアセクションに収まるものに拡張を使うべきではありません

例（Rudolfコアではなく、アダプター作者が定義するもの）：

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

コンシューマー → シミュレーター。1つのInputCommandドキュメントにつき1つのコマンドです（必要であれば、バッチ処理は将来の明示的な拡張とします）。コマンドを送る側は、SimulatorProfile.capabilitiesがサポートを宣言しているコマンドのみを送信すべきです。

```jsonc
{
  "schemaVersion": "1.0",
  "kind": "InputCommand",
  "scenarioId": "petrichor-e131-evening-2026-06-25T14:23:00Z",
  "sentAt": "2026-06-25T14:23:17.350Z",
  "sequenceNumber": 1042, // コンシューマーごとに単調増加。順序付け／冪等性のため
  "command": {
    "kind": "SetNotch",
    "value": -2,
  },
}
```

### 6.1 コマンドの種類

すべてのコマンドは`command.kind`によって判別されます。一覧：

| 種別            | ペイロード                                        | 意味                                                                                                                                                                                                                                                                              |
| --------------- | ------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `SetNotch`      | `{ value: int, relative?: bool }`                 | 統合ノッチ。`relative`（既定は`false`）= 絶対値：valueは統合ノッチ（0=N、+n=Pn、-1=抑速、-2…=B1…）。`relative: true` = 符号付きのステップ差分。いずれの場合も、`value <= -100`（センチネル`EB = -100`）は非常で、車両に依存せず、従来のハードコードされた -8 に取って代わります。 |
| `SetPowerNotch` | `{ value: int }`                                  | 力行のみの正の整数。                                                                                                                                                                                                                                                              |
| `SetBrakeNotch` | `{ value: int }`                                  | ブレーキのみの正の整数。                                                                                                                                                                                                                                                          |
| `SetBrakeSAP`   | `{ kPa: double }`                                 | 電磁直通ブレーキのSAP圧力目標。0〜400 = 常用、410 = 非常。                                                                                                                                                                                                                        |
| `SetReverser`   | `{ value: int }`                                  | レバーサー位置。`-1` = 後進、`0` = 中立、`1` = 前進。この範囲外の値は拒否しなければなりません。                                                                                                                                                                                   |
| `SetButton`     | `{ action: string, state: bool }`                 | 汎用ボタン。`action`は`VehicleAction`（§6.2）または`GameAction`（§6.3）の名前、あるいはカスタムアクションの文字列です。カスタム／仕様外のアクションは検証なしのパススルーで、`capabilities['input.button.<action>']`によってゲートされます。                                      |
| `SetWiper`      | `{ state: 'Off'\|'Intermittent'\|'Low'\|'High' }` | ワイパー位置。                                                                                                                                                                                                                                                                    |
| `SetAtoNotch`   | `{ value: int }`                                  | ATOのノッチ提案。TCのセマンティクスに従う：notch > 0のときは手動ノッチがNの場合のみ適用。notch < 0のときはmax(手動, ato) を適用。                                                                                                                                                 |
| `SetDeadman`    | `{ method: 'Hand'\|'Foot'\|'EB', holding: bool }` | チャンネルごとのデッドマンスイッチの状態。                                                                                                                                                                                                                                        |

プロデューサーは、そのように記述されたフィールドを設定しなければなりません。OPTIONALなフィールドは、コマンドごとに文書化された`default behavior`（デフォルトの挙動）に従います。

> **`SetNotch`の非常センチネル。** 予約された定数`EB = -100`（`value <= -100`のすべて）は、`relative`にかかわらず非常を要求します。生のリテラルよりこの定数を優先してください。車両に依存せず、従来のハードコードされた`-8`に取って代わります。
>
> **カスタム`SetButton`アクション。** `VehicleAction`（§6.2）と`GameAction`（§6.3）は、そのメンバーが`action`文字列にシリアライズされる仕様上の語彙です。この語彙の外にあるアクションは、専用のカスタムアクションメソッドを通じて同じ文字列フィールドを流れ、検証なしのパススルーとなります。シミュレーターは`capabilities['input.button.<action>']`で対応を宣言します。

### 6.2 VehicleAction列挙型

`SetButton`で使用する、物理的な運転台／車両の操作です。語彙はTRAIN CREW SDKに由来し、より整理された命名になっています。各エントリーには既知の意味があります。すべてのシミュレーターが対応しているとは限らないため、`SimulatorProfile.capabilities['input.button.<action>']`を確認してください。ノッチはボタンアクションではなくなりました。`SetNotch`（§6.1）を使用してください。旧`InputAction`からの改名：`Broadcast` → `InCarBroadcast`、`LightLow` → `HeadLightLow`。

- `EBReset`：EB／デッドマン警報を復帰させる（EB復帰）
- `GradientStart`：勾配起動／転動防止スイッチを作動させる（勾配起動スイッチ）
- `SafetyBrake`：保安ブレーキスイッチ（保安ブレーキ）
- `SnowBrake`：耐雪ブレーキスイッチ（耐雪ブレーキ）
- `HornAir`：空気笛を鳴らす（空気笛）
- `HornElectric`：電気笛を鳴らす（電気笛）
- `Buzzer`：合図ブザーを押す（合図ブザー）
- `BoardingPrompt`：乗降促進ブザー（乗降促進）
- `InCarBroadcast`：車内放送／PA（車内放送） — 旧`Broadcast`
- `DoorOpenLeft`：左側の客用ドアを開く（左ドア開）
- `DoorCloseLeft`：左側の客用ドアを閉じる（左ドア閉）
- `DoorOpenRight`：右側の客用ドアを開く（右ドア開）
- `DoorCloseRight`：右側の客用ドアを閉じる（右ドア閉）
- `DoorReopen`：閉扉中断後の再開閉スイッチ（再開閉SW）
- `DoorKey`：ドアスイッチ鍵の操作（ドアスイッチ鍵）
- `PartialDoor`：3/4ドア部分開スイッチ（3/4閉スイッチ）
- `DoorCut`：ドアカットスイッチ（ドアカットSW）
- `HeadLightLow`：前照灯の減光／ロービーム（前灯減光） — 旧`LightLow`
- `HeadLight`：前照灯スイッチ（前照灯SW）
- `CabinLight`：客室灯スイッチ（客室灯SW）
- `CrewRoomLight`：乗務員室灯スイッチ（乗務員室灯SW）
- `InstrumentLight`：計器灯スイッチ（計器灯SW）

### 6.3 GameAction列挙型

`SetButton`で使用する、カメラ／視点／UI／シミュレーターメタのアクションです。これらは省略可能です。コンシューマーは、いずれかが対応していることに依存すべきではありません。`SimulatorProfile.capabilities['input.button.<action>']`を確認してください。

**カメラ／視点：**

- `ExteriorView`：外部／外観視点を切り替える（外部視点切替）
- `DriverAlternateView`：運転士の別視点
- `ConductorAlternateView`：車掌の後方確認視点（後方確認）
- `LeftWindowView`：左の窓から見る
- `RightWindowView`：右の窓から見る

**シミュレーターUI／メタ：**

- `TogglePauseMenu`：ポーズメニューを切り替える
- `ToggleDiagramDisplay`：スタフ／時刻表の表示を切り替える（スタフ表示）
- `ToggleGUI`：ゲーム内UIを切り替える（画面表示）
- `ToggleCrewDoor`：乗務員ドアを切り替える
- `ToggleCrewWindow`：乗務員窓を切り替える

## 7. ワイヤー転送

Rudolfはドキュメントの形状を定義しますが、**転送方式には依存しません**。

推奨される転送方式：

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
    "transponders": null
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

### 8.3 InputCommandの例

```jsonc
// ブレーキノッチをB3へ
{ "schemaVersion": "1.0", "kind": "InputCommand", "scenarioId": "...", "sentAt": "...", "sequenceNumber": 1042,
  "command": { "kind": "SetBrakeNotch", "value": 3 } }

// 空笛を押す
{ "schemaVersion": "1.0", "kind": "InputCommand", "scenarioId": "...", "sentAt": "...", "sequenceNumber": 1043,
  "command": { "kind": "SetButton", "action": "HornAir", "state": true } }

// 空笛を放す
{ "schemaVersion": "1.0", "kind": "InputCommand", "scenarioId": "...", "sentAt": "...", "sequenceNumber": 1044,
  "command": { "kind": "SetButton", "action": "HornAir", "state": false } }
```
