namespace API.DTO
{
    public class CalendarEvent
    {
        public string Type { get; set; }
        public Guid Id { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
    }
}
