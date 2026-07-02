using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Tanuden.Rudolf.Input;

namespace Tanuden.Rudolf;

/// <summary>Contract that all Rudolf simulator adapters must satisfy.</summary>
public interface IRudolfAdapter : IDisposable
{
  /// <summary>True when the adapter has an active connection to the simulator.</summary>
  bool IsReady { get; }

  /// <summary>Start background data collection (idempotent; reconnects automatically).</summary>
  void Start(CancellationToken ct = default);

  /// <summary>
  ///   Build or return a cached <see cref="SimulatorProfile" /> for the current scenario.
  ///   Returns <c>null</c> when the simulator is not in an active scenario state.
  /// </summary>
  SimulatorProfile? GetProfile();

  /// <summary>
  ///   Build a current <see cref="OutputDataFrame" /> snapshot.
  ///   Returns <c>null</c> when the simulator is not in an active scenario state.
  /// </summary>
  OutputDataFrame? GetCurrentFrame();

  /// <summary>Forward a Rudolf command to the simulator.</summary>
  void Dispatch(Command command);

  /// <summary>
  ///   Adapter-specific supplemental data: status, diagnostics, extension payloads.
  ///   Keys use the same namespaced convention as <see cref="OutputDataFrame.Extensions" />.
  ///   A minimal adapter may return an empty dictionary.
  /// </summary>
  IReadOnlyDictionary<string, JsonElement> GetAdapterExtensions();
}
