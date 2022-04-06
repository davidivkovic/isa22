namespace api.Core.Models;

public class Review : Entity
{
    public User     User      { get; init; }
    public Business Business  { get; init; }
    public double   Rating    { get; init; }
    public string   Content   { get; init; }
    public DateTime Timestamp { get; init; }
    public bool     Approved  { get; private set; }

    public void Approve()
    {
        Approved = true;
    }
}
