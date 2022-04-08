namespace API.Infrastructure.Extensions;

public static class GuidExtensions
{
    public static Guid ToGuid(this string id)
    {
        return Guid.TryParse(id, out Guid parsed) ? parsed : Guid.Empty;
    }
}