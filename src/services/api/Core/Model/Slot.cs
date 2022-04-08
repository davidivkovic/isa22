namespace API.Core.Model;

public class Slot : Entity
{
    public DateTime Start     { get; set; }
    public DateTime End       { get; set; }
    public bool     Available { get; set; }

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