namespace api.Core.Models;

public class DeletionRequest
{
    public string Reason   { get; private set; }
    public string Answer   { get; private set; }
    public bool   Approved { get; private set; }

    public void Approve(string answer)
    {
        Answer = answer;
        Approved = true;
    }

    public void Deny(string answer)
    {
        Answer = answer;
        Approved = false;
    }
}

public class Penalty
{
    public int      Points  { get; private set; }
    public DateTime Expires { get; private set; }

    private void Unexpire()
    {
        Expires = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.Day + 1);
    }

    public void Increment()
    {
        Points++;
        Unexpire();
    }

    public void Reset()
    {
        Points = 0;
        Unexpire();
    }
}

public class User : Entity
{   
    public string          FirstName       { get; set; }
    public string          LastName        { get; set; }
    public string          Email           { get; set; }
    public string          Password        { get; set; }
    public Address         Address         { get; set; }
    public Penalty         Penalty         { get; set; }
    public int             LoyaltyPoints   { get; set; }
    public Loyalty         Level           { get; set; }
    public DeletionRequest DeletionRequest { get; set; }
    public List<Business>  Subscriptions   { get; set; } = new();

    // Add new deletion on request
}
