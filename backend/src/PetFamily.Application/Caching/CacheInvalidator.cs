namespace PetFamily.Application.Caching;

public sealed class CacheInvalidator(ICacheService cacheService) : ICacheInvalidator
{
    public async Task InvalidateVolunteerAsync(
        Guid volunteerId,
        CancellationToken cancellationToken = default)
    {
        await cacheService.RemoveAsync(CacheKeys.Volunteer(volunteerId), cancellationToken);
        await cacheService.RemoveByPrefixAsync(CacheKeys.VolunteersAllPrefix, cancellationToken);

        // PetDto содержит имя волонтёра, поэтому списки и карточки питомцев тоже устаревают.
        await cacheService.RemoveByPrefixAsync(CacheKeys.PetsPrefix, cancellationToken);
    }

    public async Task InvalidatePetAsync(
        Guid petId,
        Guid volunteerId,
        CancellationToken cancellationToken = default)
    {
        await cacheService.RemoveAsync(CacheKeys.Pet(petId), cancellationToken);
        await cacheService.RemoveByPrefixAsync(CacheKeys.PetsAllPrefix, cancellationToken);

        // У волонтёра меняются счётчики питомцев.
        await cacheService.RemoveAsync(CacheKeys.Volunteer(volunteerId), cancellationToken);
        await cacheService.RemoveByPrefixAsync(CacheKeys.VolunteersAllPrefix, cancellationToken);
    }
}
