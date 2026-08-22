using PetFamily.Application;
using PetFamily.Domain.VolunteerManagement;

namespace PetFamily.Infrastructure.Repositories;



public class VolunteersRepository(ApplicationDbContext context) : IVolunteerRepository
{
    public async Task<Guid> Add(Volunteer volunteer, CancellationToken token = default)
    {
        await context.Volunteers.AddAsync(volunteer, token);
        await context.SaveChangesAsync(token);
        
        return volunteer.Id.Value; //смотря че возвращать надо хз
    }
    
    // public GetByPhoneNumber
}