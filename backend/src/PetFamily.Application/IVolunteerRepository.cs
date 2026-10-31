using PetFamily.Domain.Shared.DifferentId;
using PetFamily.Domain.VolunteerManagement;

namespace PetFamily.Application;

public interface IVolunteerRepository
{
    Task<Guid> Add(Volunteer volunteer, CancellationToken token = default);

    Task<Volunteer?> GetById(
        IdVolunteer id,
        bool includePets = false,
        CancellationToken cancellationToken = default);

    Task SaveChanges(CancellationToken cancellationToken = default);
}
