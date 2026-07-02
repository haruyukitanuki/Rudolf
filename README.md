# Railway Unified Display Object Link Format (Rudolf)

**English** | [日本語](./README.ja.md)

An open source wire-format standard for exchanging train state and commands between Japanese train simulators and external programs/mods.

It is designed to serve any Japanese train simulator, past, future and present.

Rudolf is made with the consideration:

- Agnostic: Not shaped to any particular simulator's data schema. Unsupport fields are nullable. Method of data population is unprescribed.
- Ideal for HMIs/TIMS/MON: Baseline standard is enough to drive instrument displays.
- Bidirectionality: Both schemas for input and output are defined for simulator rigs.
- Extensibility: Sim-specific extensions blocks can be used when providing additional data. Vocabulary maps are available for programs to use for overriding based on route.

### Why deprecate OpenTetsu & create Rudolf?

When OpenTetsu was developed, it was created as a improved and organised way to transmit TRAINCREW (TC) telemetry as JSON over websockets. As the format was created purely with only TC in mind, it is not extensible and easily adaptable to other simulators despite its purpose of supposedly being one.

I decided to deprecate OpenTetsu for that reason and create another format, called Rudolf (now there are 2 competing standards lol).

<figure>
  <img src="./docs/standards.png" alt='"Standards"'>
  <figcaption>"Standards" by <a href="https://xkcd.com/927/">xkcd.com</a></figcaption>
</figure>

## Packages

| Language   | Package                           | Location                            |
| ---------- | --------------------------------- | ----------------------------------- |
| TypeScript | [`@tanuden/rudolf`](./typescript) | `npm install @tanuden/rudolf`       |
| C#/.NET    | [`Tanuden.Rudolf`](./dotnet)      | `dotnet add package Tanuden.Rudolf` |

Both packages contain only the type definitions.
There is no runtime validation, transport or logic.

## Adapter Packages

This repository only contains the structure of wire format. Please refer to [Rudolf Adapter Repository](https://github.com/haruyukitanuki/Rudolf.Adapters).

Otherwise, you can always write your own if none is present for your use case!

## Specification

The canonical spec lives at [`spec/rudolf-spec.md`](./spec/rudolf-spec.md).

For migrations, please refer to [`docs/opentetsu-rudolf-migration.md`](./docs/opentetsu-rudolf-migration.md) for rough field equivilents.

## R-rudolf?

It was supposed to be RUDF but I kept reading it as _Rudolf_ when it gets katakana'ifed.
And yes, Uma. _Symboli Rudolf_.

## 💾 Open Source @ Tanuden

Rudolf is Open Source Software (OSS), licensed under Apache 2.0. You may freely distribute, use and modify code provided to you in repository in accordance with it.

A copy of the license can be found at the root of the repository [here](https://github.com/haruyukitanuki/rudolf/blob/main/LICENSE.md).

## 💝 Support

[Tanuden Discord Server](https://go.tanu.ch/tanuden-discord) | [Twitter](https://go.tanu.ch/twitter) | [YouTube](https://go.tanu.ch/tanutube)

**Tanukigawa Railway | Copyright (c) 2026 Haruyuki Tanukiji.**
