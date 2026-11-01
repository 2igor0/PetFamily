using PetFamily.Application.Dtos;
using PetFamily.Application.modules.GetPets;

namespace PetFamily.Infrastructure.Read;

public interface IPetsReadRepository
{
    Task<PagedResult<PetDto>> GetAll(
        GetPetsRequest request,
        CancellationToken cancellationToken = default);

    Task<PetDetailsDto?> GetById(
        Guid id,
        CancellationToken cancellationToken = default);
}
