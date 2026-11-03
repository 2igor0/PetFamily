using NSubstitute;
using PetFamily.Application.Caching;
using PetFamily.Application.Dtos;
using PetFamily.Application.modules.GetVolunteers;
using PetFamily.Infrastructure.Read;

namespace PetFamily.UnitTests;

public class CachedVolunteersReadRepositoryTests
{
    private readonly IVolunteersReadRepository _inner = Substitute.For<IVolunteersReadRepository>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly CachedVolunteersReadRepository _repository;

    public CachedVolunteersReadRepositoryTests()
    {
        _repository = new CachedVolunteersReadRepository(_inner, _cache);
    }

    [Fact]
    public async Task GetById_WhenCached_ReturnsCachedValueWithoutCallingInner()
    {
        var volunteerId = Guid.NewGuid();
        var cachedVolunteer = new VolunteerDetailsDto { Id = volunteerId, FirstName = "Иван" };

        _cache
            .GetAsync<VolunteerDetailsDto>(
                CacheKeys.Volunteer(volunteerId),
                Arg.Any<CancellationToken>())
            .Returns(cachedVolunteer);

        var result = await _repository.GetById(volunteerId);

        Assert.Same(cachedVolunteer, result);
        await _inner.DidNotReceive().GetById(
            Arg.Any<Guid>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetById_WhenNotCached_CallsInnerAndStoresValue()
    {
        var volunteerId = Guid.NewGuid();
        var volunteer = new VolunteerDetailsDto { Id = volunteerId, FirstName = "Иван" };

        _cache
            .GetAsync<VolunteerDetailsDto>(
                CacheKeys.Volunteer(volunteerId),
                Arg.Any<CancellationToken>())
            .Returns((VolunteerDetailsDto?)null);

        _inner
            .GetById(volunteerId, Arg.Any<CancellationToken>())
            .Returns(volunteer);

        var result = await _repository.GetById(volunteerId);

        Assert.Same(volunteer, result);
        await _cache.Received(1).SetAsync(
            CacheKeys.Volunteer(volunteerId),
            volunteer,
            CacheSettings.DetailTtl,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAll_UsesCacheKeyWithPagination()
    {
        var request = new GetVolunteersRequest(Page: 2, PageSize: 5);
        var pagedResult = new PagedResult<VolunteerDto>([], 2, 5, 0);

        _cache
            .GetOrCreateAsync(
                CacheKeys.VolunteersAll(2, 5),
                Arg.Any<Func<CancellationToken, Task<PagedResult<VolunteerDto>>>>(),
                CacheSettings.ListTtl,
                Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var result = await _repository.GetAll(request);

        Assert.Same(pagedResult, result);
        await _inner.DidNotReceive().GetAll(
            Arg.Any<GetVolunteersRequest>(),
            Arg.Any<CancellationToken>());
    }
}
