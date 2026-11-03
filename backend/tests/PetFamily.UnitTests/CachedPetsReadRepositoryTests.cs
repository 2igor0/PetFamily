using NSubstitute;
using PetFamily.Application.Caching;
using PetFamily.Application.Dtos;
using PetFamily.Application.modules.GetPets;
using PetFamily.Infrastructure.Read;

namespace PetFamily.UnitTests;

public class CachedPetsReadRepositoryTests
{
    private readonly IPetsReadRepository _inner = Substitute.For<IPetsReadRepository>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly CachedPetsReadRepository _repository;

    public CachedPetsReadRepositoryTests()
    {
        _repository = new CachedPetsReadRepository(_inner, _cache);
    }

    [Fact]
    public async Task GetById_WhenCached_ReturnsCachedValueWithoutCallingInner()
    {
        var petId = Guid.NewGuid();
        var cachedPet = new PetDetailsDto { Id = petId, Name = "Барсик" };

        _cache
            .GetAsync<PetDetailsDto>(CacheKeys.Pet(petId), Arg.Any<CancellationToken>())
            .Returns(cachedPet);

        var result = await _repository.GetById(petId);

        Assert.Same(cachedPet, result);
        await _inner.DidNotReceive().GetById(
            Arg.Any<Guid>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetById_WhenNotCached_CallsInnerAndStoresValue()
    {
        var petId = Guid.NewGuid();
        var pet = new PetDetailsDto { Id = petId, Name = "Барсик" };

        _cache
            .GetAsync<PetDetailsDto>(CacheKeys.Pet(petId), Arg.Any<CancellationToken>())
            .Returns((PetDetailsDto?)null);

        _inner
            .GetById(petId, Arg.Any<CancellationToken>())
            .Returns(pet);

        var result = await _repository.GetById(petId);

        Assert.Same(pet, result);
        await _cache.Received(1).SetAsync(
            CacheKeys.Pet(petId),
            pet,
            CacheSettings.DetailTtl,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAll_WithoutFilters_UsesCache()
    {
        var request = new GetPetsRequest(Page: 1, PageSize: 10);
        var pagedResult = new PagedResult<PetDto>([], 1, 10, 0);

        _cache
            .GetOrCreateAsync(
                CacheKeys.PetsAll(1, 10, request.SortBy, request.SortDirection),
                Arg.Any<Func<CancellationToken, Task<PagedResult<PetDto>>>>(),
                CacheSettings.ListTtl,
                Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var result = await _repository.GetAll(request);

        Assert.Same(pagedResult, result);
        await _inner.DidNotReceive().GetAll(
            Arg.Any<GetPetsRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAll_WithFilter_BypassesCache()
    {
        var request = new GetPetsRequest(Name: "Барсик");
        var pagedResult = new PagedResult<PetDto>([], 1, 10, 0);

        _inner
            .GetAll(request, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var result = await _repository.GetAll(request);

        Assert.Same(pagedResult, result);
        await _cache.DidNotReceive().GetOrCreateAsync(
            Arg.Any<string>(),
            Arg.Any<Func<CancellationToken, Task<PagedResult<PetDto>>>>(),
            Arg.Any<TimeSpan>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAll_WithOnlySortAndPagination_UsesCache()
    {
        var request = new GetPetsRequest(
            Page: 2,
            PageSize: 5,
            SortBy: "Name",
            SortDirection: "Asc");

        var pagedResult = new PagedResult<PetDto>([], 2, 5, 0);

        _cache
            .GetOrCreateAsync(
                CacheKeys.PetsAll(2, 5, "Name", "Asc"),
                Arg.Any<Func<CancellationToken, Task<PagedResult<PetDto>>>>(),
                CacheSettings.ListTtl,
                Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var result = await _repository.GetAll(request);

        Assert.Same(pagedResult, result);
        await _inner.DidNotReceive().GetAll(
            Arg.Any<GetPetsRequest>(),
            Arg.Any<CancellationToken>());
    }
}
