namespace api.Core.Models;

public abstract class Entity
{
    public Guid Id        { get; set; }
    public bool IsDeleted { get; private set; }

    public void Delete()
    {
        IsDeleted = true;
    }
}