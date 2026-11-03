namespace PetFamily.Application.Caching;

public interface ICacheInvalidator
{
    Task InvalidateVolunteerAsync(
        Guid volunteerId,
        CancellationToken cancellationToken = default);

    Task InvalidatePetAsync(
        Guid petId,
        Guid volunteerId,
        CancellationToken cancellationToken = default);
}
