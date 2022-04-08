namespace API.Core.Model;

public interface IDeletable
{
    public bool IsDeleted { get; set; }
    public void Delete();
}

public abstract class Entity : IDeletable
{
    public Guid Id        { get; set; }
    public bool IsDeleted { get; set; }

    public void Delete() => IsDeleted = true;
}