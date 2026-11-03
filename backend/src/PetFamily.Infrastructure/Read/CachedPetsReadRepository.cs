using PetFamily.Application.Caching;
using PetFamily.Application.Dtos;
using PetFamily.Application.modules.GetPets;

namespace PetFamily.Infrastructure.Read;

public sealed class CachedPetsReadRepository(
    IPetsReadRepository inner,
    ICacheService cacheService) : IPetsReadRepository
{
    public Task<PagedResult<PetDto>> GetAll(
        GetPetsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (HasFilters(request))
            return inner.GetAll(request, cancellationToken);

        var cacheKey = CacheKeys.PetsAll(
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortDirection);

        return cacheService.GetOrCreateAsync(
            cacheKey,
            token => inner.GetAll(request, token),
            CacheSettings.ListTtl,
            cancellationToken);
    }

    public async Task<PetDetailsDto?> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.Pet(id);
        var cachedPet = await cacheService.GetAsync<PetDetailsDto>(cacheKey, cancellationToken);

        if (cachedPet is not null)
            return cachedPet;

        var pet = await inner.GetById(id, cancellationToken);

        if (pet is not null)
            await cacheService.SetAsync(cacheKey, pet, CacheSettings.DetailTtl, cancellationToken);

        return pet;
    }

    private static bool HasFilters(GetPetsRequest request)
    {
        return !string.IsNullOrWhiteSpace(request.Name)
               || !string.IsNullOrWhiteSpace(request.Species)
               || !string.IsNullOrWhiteSpace(request.Breed)
               || !string.IsNullOrWhiteSpace(request.Color)
               || !string.IsNullOrWhiteSpace(request.City)
               || !string.IsNullOrWhiteSpace(request.Country)
               || request.AgeFrom.HasValue
               || request.AgeTo.HasValue
               || !string.IsNullOrWhiteSpace(request.HelpStatus)
               || request.VolunteerId.HasValue;
    }
}
