using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TimerUtil
{
    public static bool DisableTimers = false;
    private static Dictionary<string, TimerUtil> Timers = new Dictionary<string, TimerUtil>();
    private long StartMillis;
    private long TotalMillis;
    private string Label;
    private int TimesStarted = 0;

    public static void StartTimer(string label)
    {
        if (DisableTimers) return;
        Timers[label].Start();
    } 

    public static void ResetTimers() => Timers.Clear();

    public static void StartTrial(params string[] labels)
    {
        if (DisableTimers) return;
        foreach (string label in labels)
        {
            if (!Timers.TryGetValue(label, out TimerUtil timer))
            {
                timer = new TimerUtil(label);
                Timers[label] = timer;
            }
            timer.TimesStarted++;
        }
    }

    public static void StopTimer(string label)
    {
        if (DisableTimers) return;
        Timers[label].StopTimer();
    }

    public static string ReportAllTimers()
    {
        if (DisableTimers)
        {
            return "Timers disabled.";
        }
        StringBuilder builder = new StringBuilder();

        string format = "{0,25} | {1,10} | {2,10} | {3,10}\n";
        builder.Append(string.Format(format, "Name", "Trials", "Total", "Average"));
        var results = Timers.Select(timer =>
            string.Format(format,
                          timer.Key,
                          timer.Value.TimesStarted,
                          timer.Value.TotalMillis,
                          $"{(float)(timer.Value.TotalMillis) / timer.Value.TimesStarted:000.0}")).ToArray();
        foreach (string result in results)
            builder.Append(result);
        return "\n" + builder.ToString();
    }

    public TimerUtil(string label)
    {
        this.Label = label;
    }
    public long Start()
    {
        this.StartMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        return this.TotalMillis;
    }

    public long StopTimer()
    {
        long stopTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        long elapsed = stopTime - this.StartMillis;
        this.TotalMillis += elapsed;
        return this.TotalMillis;
    }

    public string ReportElapsed()
    {
        long cTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        return $"Time Elapsed({this.Label}): {cTime - StartMillis}.";
    }
}