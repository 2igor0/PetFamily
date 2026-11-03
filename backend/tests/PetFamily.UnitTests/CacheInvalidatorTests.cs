using NSubstitute;
using PetFamily.Application.Caching;

namespace PetFamily.UnitTests;

public class CacheInvalidatorTests
{
    private readonly ICacheService _cache = Substitute.For<ICacheService>();
    private readonly CacheInvalidator _invalidator;

    public CacheInvalidatorTests()
    {
        _invalidator = new CacheInvalidator(_cache);
    }

    [Fact]
    public async Task InvalidateVolunteerAsync_RemovesVolunteerAndRelatedCaches()
    {
        var volunteerId = Guid.NewGuid();

        await _invalidator.InvalidateVolunteerAsync(volunteerId);

        await _cache.Received(1).RemoveAsync(
            CacheKeys.Volunteer(volunteerId),
            Arg.Any<CancellationToken>());

        await _cache.Received(1).RemoveByPrefixAsync(
            CacheKeys.VolunteersAllPrefix,
            Arg.Any<CancellationToken>());

        await _cache.Received(1).RemoveByPrefixAsync(
            CacheKeys.PetsPrefix,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InvalidatePetAsync_RemovesPetAndVolunteerCaches()
    {
        var petId = Guid.NewGuid();
        var volunteerId = Guid.NewGuid();

        await _invalidator.InvalidatePetAsync(petId, volunteerId);

        await _cache.Received(1).RemoveAsync(
            CacheKeys.Pet(petId),
            Arg.Any<CancellationToken>());

        await _cache.Received(1).RemoveByPrefixAsync(
            CacheKeys.PetsAllPrefix,
            Arg.Any<CancellationToken>());

        await _cache.Received(1).RemoveAsync(
            CacheKeys.Volunteer(volunteerId),
            Arg.Any<CancellationToken>());

        await _cache.Received(1).RemoveByPrefixAsync(
            CacheKeys.VolunteersAllPrefix,
            Arg.Any<CancellationToken>());
    }
}
