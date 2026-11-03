using PetFamily.Application.Caching;
using PetFamily.Application.Dtos;
using PetFamily.Application.modules.GetVolunteers;

namespace PetFamily.Infrastructure.Read;

public sealed class CachedVolunteersReadRepository(
    IVolunteersReadRepository inner,
    ICacheService cacheService) : IVolunteersReadRepository
{
    public Task<PagedResult<VolunteerDto>> GetAll(
        GetVolunteersRequest request,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.VolunteersAll(request.Page, request.PageSize);

        return cacheService.GetOrCreateAsync(
            cacheKey,
            token => inner.GetAll(request, token),
            CacheSettings.ListTtl,
            cancellationToken);
    }

    public async Task<VolunteerDetailsDto?> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.Volunteer(id);
        var cachedVolunteer = await cacheService.GetAsync<VolunteerDetailsDto>(
            cacheKey,
            cancellationToken);

        if (cachedVolunteer is not null)
            return cachedVolunteer;

        var volunteer = await inner.GetById(id, cancellationToken);

        if (volunteer is not null)
            await cacheService.SetAsync(cacheKey, volunteer, CacheSettings.DetailTtl, cancellationToken);

        return volunteer;
    }
}
