namespace Tanuden.Rudolf.Sections;

/// <summary>Sim clock and frame counter for an <see cref="Tanuden.Rudolf.OutputDataFrame" />.</summary>
public class Time
{
  /// <summary>ISO local datetime (no time zone).</summary>
  public string Sim = "00:00:00";

  /// <summary>Seconds since scenario start; monotonic.</summary>
  public double Elapsed;

  /// <summary>Frame counter; increments each emit.</summary>
  public long Tick;
}
