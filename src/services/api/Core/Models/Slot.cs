namespace api.Core.Models;

public class Slot
{
    public DateTime Start     { get; private set; }
    public DateTime End       { get; private set; }
    public bool     Available { get; private set; }

    public bool Contains(DateTime start, DateTime end)
    {
        return Start <= start && End >= end;
    }

    public bool Intersects(DateTime start, DateTime end)
    {
        return Contains(start, end)           ||
               Start >= start && Start <= end ||
               End >= start && End <= end;

    }
}