using PetFamily.Application.Dtos;
using PetFamily.Application.modules.GetVolunteers;

namespace PetFamily.Infrastructure.Read;

public interface IVolunteersReadRepository
{
    Task<PagedResult<VolunteerDto>> GetAll(
        GetVolunteersRequest request,
        CancellationToken cancellationToken = default);

    Task<VolunteerDetailsDto?> GetById(
        Guid id,
        CancellationToken cancellationToken = default);
}
