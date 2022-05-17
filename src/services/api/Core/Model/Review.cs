namespace API.Core.Model;

public class Review : Entity
{
    public User           User      { get; set; }
    public Business       Business  { get; set; }
    public double         Rating    { get; set; }
    public string         Content   { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public bool           Approved  { get; set; }
    public bool           Rejected  { get; set; }

    public void Approve()
    {
        Approved = true;
    }

    public void Deny()
    {
        Rejected = true;
    }
}