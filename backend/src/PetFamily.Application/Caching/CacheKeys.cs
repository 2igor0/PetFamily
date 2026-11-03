namespace PetFamily.Application.Caching;

public static class CacheKeys
{
    private const string Prefix = "petfamily:";

    public const string VolunteersAllPrefix = Prefix + "volunteers:all:";

    public const string PetsAllPrefix = Prefix + "pets:all:";

    public const string PetsPrefix = Prefix + "pets:";

    public static string Volunteer(Guid volunteerId) => $"{Prefix}volunteers:{volunteerId}";

    public static string VolunteersAll(int page, int pageSize) =>
        $"{VolunteersAllPrefix}{page}:{pageSize}";

    public static string Pet(Guid petId) => $"{PetsPrefix}{petId}";

    public static string PetsAll(
        int page,
        int pageSize,
        string? sortBy,
        string? sortDirection) =>
        $"{PetsAllPrefix}{page}:{pageSize}:{sortBy ?? "CreatedAt"}:{sortDirection ?? "Desc"}";
}

public static class CacheSettings
{
    public static readonly TimeSpan DetailTtl = TimeSpan.FromMinutes(10);

    public static readonly TimeSpan ListTtl = TimeSpan.FromMinutes(5);
}
