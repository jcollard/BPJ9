using System;

public class TimerUtil
{
    private long StartMillis;
    private string Label;

    public static TimerUtil StartTimer(string label)
    {
        TimerUtil tu = new TimerUtil(label);
        tu.Start();
        return tu;
    }

    public TimerUtil(string label)
    {
        this.Label = label;
    }
    public void Start()
    {
        this.StartMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    public string ReportElapsed()
    {
        long cTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        return $"Time Elapsed({this.Label}): {cTime-StartMillis}.";
    }
}