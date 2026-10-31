using Microsoft.EntityFrameworkCore;
using PetFamily.Application;
using PetFamily.Domain.Shared.DifferentId;
using PetFamily.Domain.VolunteerManagement;

namespace PetFamily.Infrastructure.Repositories;

public class VolunteersRepository(ApplicationDbContext context) : IVolunteerRepository
{
    public async Task<Guid> Add(Volunteer volunteer, CancellationToken token = default)
    {
        await context.Volunteers.AddAsync(volunteer, token);
        await context.SaveChangesAsync(token);

        return volunteer.Id.Value;
    }

    public async Task<Volunteer?> GetById(
        IdVolunteer id,
        bool includePets = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Volunteer> query = context.Volunteers;

        if (includePets)
            query = query.Include(volunteer => volunteer.Pets);

        return await query.FirstOrDefaultAsync(volunteer => volunteer.Id == id, cancellationToken);
    }

    public Task SaveChanges(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
