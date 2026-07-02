# Tanuden.Rudolf

C# type definitions for the **RUDOLF** (Railway Unified Display Object Link Format) wire schema.

## Install

```bash
dotnet add package Tanuden.Rudolf
```

## Use

```csharp
using System.Text.Json;
using Tanuden.Rudolf;
using Tanuden.Rudolf.Json;

var frame = JsonSerializer.Deserialize<OutputFrame>(json, RudolfJson.Options);
var speed = frame.Physics.Speed;
```

This package contains **only** DTOs and a default `JsonSerializerOptions`. No runtime validation, no transport, no
business logic. Target framework is `netstandard2.0` so it works with .NET Framework 4.8 (for BveEx plugins) and modern
.NET.

## Specification

See [the RUDOLF spec](https://github.com/tanuden/rudolf/blob/main/spec/rudolf-spec.md) for the full wire format.

## License

Apache 2.0.
