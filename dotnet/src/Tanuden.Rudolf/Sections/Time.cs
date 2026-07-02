namespace Tanuden.Rudolf.Sections;

/// <summary>Sim clock and frame counter for an <see cref="Tanuden.Rudolf.OutputDataFrame" />.</summary>
public class Time
{
  /// <summary><c>"HH:MM:SS"</c> when <see cref="DateKnown" /> is false; ISO datetime when true.</summary>
  public string Sim = "00:00:00";

  /// <summary>True when the sim provides a real date (most train sims do not).</summary>
  public bool DateKnown;

  /// <summary>Seconds since scenario start; monotonic.</summary>
  public double Elapsed;

  /// <summary>Frame counter; increments each emit.</summary>
  public long Tick;
}
