# Railway Unified Display Object Link Format (Rudolf)

[English](./README.md) | **日本語**

日本の列車シミュレーターと外部プログラム／Modとの間で、列車の状態やコマンドをやり取りするためのオープンソースのワイヤーフォーマット規格です。

過去・現在・未来を問わず、あらゆる日本の列車シミュレーターで利用できるよう設計されています。

Rudolfは次の点を考慮して作られています。

- 非依存（Agnostic）：特定のシミュレーターのデータスキーマに合わせて設計されていません。サポートされないフィールドはnull許容（nullable）です。データの投入方法についても規定しません。
- HMI／TIMS／MONに最適：基本仕様だけで計器類のディスプレイを駆動できます。
- 双方向性：シミュレーター機器向けに、入力用と出力用の両方のスキーマを定義しています。
- 拡張性：追加データを提供したい場合は、シミュレーター固有の拡張ブロックを利用できます。また、プログラムが路線に応じて値を上書きするために使える語彙マップ（Vocabulary map）も用意されています。

### なぜOpenTetsuを非推奨にしてRudolfを作ったのか？

OpenTetsuはもともと、TRAINCREW（TC）のテレメトリをWebSocket経由でJSONとして送信するための、改良され整理された手段として開発されました。しかし、このフォーマットはTCだけを念頭に置いて作られていたため、本来は汎用的であるはずなのに拡張性に乏しく、他のシミュレーターへ柔軟に対応させることが難しいものでした。

そうした理由から、私はOpenTetsuを非推奨とし、Rudolfという別のフォーマットを作ることにしました（これで競合する標準が2つに増えてしまいましたね(笑)）。

<figure>
  <img src="./docs/standards.png" alt='"Standards"'>
  <figcaption><a href="https://xkcd.com/927/">xkcd.com</a> による "Standards"</figcaption>
</figure>

## パッケージ

| 言語       | パッケージ                        | インストール                        |
| ---------- | --------------------------------- | ----------------------------------- |
| TypeScript | [`@tanuden/rudolf`](https://github.com/haruyukitanuki/rudolf-ts) | 近日公開       | <!-- npm install @tanuden/rudolf -->
| C#/.NET    | [`Tanuden.Rudolf`](./dotnet)      | 近日公開 | <!-- dotnet add package Tanuden.Rudolf -->

どちらのパッケージにも型定義のみが含まれています。
実行時のバリデーションやトランスポート、ロジックは含まれていません。

## アダプターパッケージ

このリポジトリにはワイヤーフォーマットの構造のみが含まれています。実装については[Rudolfアダプターリポジトリ](https://github.com/haruyukitanuki/Rudolf.Adapters) を参照してください。

もし目的に合うアダプターがなければ、いつでも自分で実装できます！

## 仕様

正式な仕様は [`spec/rudolf-spec.md`](./spec/rudolf-spec.md)にあります。

移行の際は、フィールドのおおまかな対応関係について [`docs/opentetsu-rudolf-migration.md`](./docs/opentetsu-rudolf-migration.md) を参照してください。

## ルドルフの名称ってなんで？

本当はRUDFにするつもりだったのですが、カタカナにするとどうしても「ルドルフ」と読んでしまうんですよね。
そう、ウマ娘です。「シンボリルドルフ」。

## 💾 オープンソース @ タヌ電

RudolfはApache 2.0ライセンスのオープンソースソフトウェア（OSS）です。ライセンスに従う限り、このリポジトリで提供されるコードを自由に配布・利用・改変できます。

ライセンスの全文はリポジトリのルート（[こちら](https://github.com/haruyukitanuki/rudolf/blob/main/LICENSE.md)）にあります。

## 💝 サポート

[タヌ電Discordサーバー](https://go.tanu.ch/tanuden-discord) | [Twitter](https://go.tanu.ch/twitter) | [YouTube](https://go.tanu.ch/tanutube)

**狸河電鉄 | Copyright (c) 2026 狸治はるゆき.**
