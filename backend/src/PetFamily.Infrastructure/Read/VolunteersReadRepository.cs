using Dapper;
using PetFamily.Application.Dtos;
using PetFamily.Application.modules.GetVolunteers;

namespace PetFamily.Infrastructure.Read;

public sealed class VolunteersReadRepository(IDbConnectionFactory connectionFactory)
    : IVolunteersReadRepository
{
    private const string VolunteerFields = """
        v.id AS Id,
        v.first_name AS FirstName,
        v.last_name AS LastName,
        v.patronymic AS Patronymic,
        v.email AS Email,
        v.description AS Description,
        v.years_of_experience AS YearsOfExperience,
        v.phone_number AS PhoneNumber,
        v.created_at AS CreatedAt,
        (SELECT COUNT(*)::int
         FROM pets p
         WHERE p.volunteer_id = v.id
           AND p.is_deleted = false
           AND p.help_status = 'FoundHome') AS PetsFoundHomeCount,
        (SELECT COUNT(*)::int
         FROM pets p
         WHERE p.volunteer_id = v.id
           AND p.is_deleted = false
           AND p.help_status = 'LookingForHome') AS PetsSeekingHomeCount,
        (SELECT COUNT(*)::int
         FROM pets p
         WHERE p.volunteer_id = v.id
           AND p.is_deleted = false
           AND p.help_status = 'NeedsHelp') AS PetsUnderTreatmentCount
        """;

    public async Task<PagedResult<VolunteerDto>> GetAll(
        GetVolunteersRequest request,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        const string totalSql = "SELECT COUNT(*)::int FROM volunteers WHERE is_deleted = false";

        var totalCount = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(totalSql, cancellationToken: cancellationToken));

        var sql = $"""
            SELECT {VolunteerFields}
            FROM volunteers v
            WHERE v.is_deleted = false
            ORDER BY v.created_at DESC
            LIMIT @PageSize OFFSET @Offset
            """;

        var items = await connection.QueryAsync<VolunteerDto>(
            new CommandDefinition(
                sql,
                new
                {
                    request.PageSize,
                    Offset = (request.Page - 1) * request.PageSize
                },
                cancellationToken: cancellationToken));

        return new PagedResult<VolunteerDto>(
            items.ToList(),
            request.Page,
            request.PageSize,
            totalCount);
    }

    public async Task<VolunteerDetailsDto?> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        var sql = $"""
            SELECT {VolunteerFields}
            FROM volunteers v
            WHERE v.id = @Id AND v.is_deleted = false
            """;

        var volunteer = await connection.QuerySingleOrDefaultAsync<VolunteerDetailsDto>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        if (volunteer is null)
            return null;

        const string socialLinksSql = """
            SELECT name AS Name, url AS Url
            FROM volunteer_social_networks
            WHERE volunteer_id = @Id
            ORDER BY id
            """;

        const string donationDetailsSql = """
            SELECT name AS Name, description AS Description
            FROM volunteer_donation_details
            WHERE volunteer_id = @Id
            ORDER BY id
            """;

        var socialLinks = await connection.QueryAsync<SocialLinkRow>(
            new CommandDefinition(socialLinksSql, new { Id = id }, cancellationToken: cancellationToken));

        var donationDetails = await connection.QueryAsync<DonationDetailsRow>(
            new CommandDefinition(donationDetailsSql, new { Id = id }, cancellationToken: cancellationToken));

        volunteer.SocialLinks = socialLinks
            .Select(link => new SocialLinkDto(link.Name, link.Url))
            .ToList();

        volunteer.DonationDetails = donationDetails
            .Select(details => new DonationsRequisites(details.Name, details.Description))
            .ToList();

        return volunteer;
    }

    private sealed class SocialLinkRow
    {
        public string Name { get; set; } = null!;

        public string Url { get; set; } = null!;
    }

    private sealed class DonationDetailsRow
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;
    }
}
