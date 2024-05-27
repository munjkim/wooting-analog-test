using System.Diagnostics;

Console.WriteLine($"Timer frequency in ticks per second = {Stopwatch.Frequency}");
double resolution = 1.0 / Stopwatch.Frequency;
Console.WriteLine($"Timer resolution in seconds = {resolution}");