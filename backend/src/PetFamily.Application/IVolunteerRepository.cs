using PetFamily.Domain.VolunteerManagement;

namespace PetFamily.Application;

public interface IVolunteerRepository // репоситорий создать
{
    Task<Guid> Add(Volunteer volunteer, CancellationToken token = default);
}