namespace API.Core.Model;

public class Review : Entity
{
    public User     User      { get; set; }
    public Business Business  { get; set; }
    public double   Rating    { get; set; }
    public string   Content   { get; set; }
    public DateTime Timestamp { get; set; }
    public bool     Approved  { get; set; }

    public void Approve()
    {
        Approved = true;
    }
}
