namespace API.Core.Model;

public class Slot : Entity
{
    public DateTimeOffset Start     { get; set; }
    public DateTimeOffset End       { get; set; }
    public bool           Available { get; set; }

    public bool Contains(DateTimeOffset start, DateTimeOffset end)
    {
        return Start <= start && End >= end;
    }

    public bool Intersects(DateTimeOffset start, DateTimeOffset end)
    {
        return Contains(start, end)           ||
               Start >= start && Start <= end ||
               End >= start && End <= end;
    }
}