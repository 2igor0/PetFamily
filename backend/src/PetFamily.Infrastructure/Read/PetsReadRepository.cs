using System.Text;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Dtos;
using PetFamily.Application.FileStorage;
using PetFamily.Application.modules.GetPets;
using PetFamily.Domain.PetManagement.Enums;

namespace PetFamily.Infrastructure.Read;

public sealed class PetsReadRepository(
    IDbConnectionFactory connectionFactory,
    IFileStorage fileStorage,
    ILogger<PetsReadRepository> logger) : IPetsReadRepository
{
    private const string FromClause = """
        FROM pets p
        JOIN volunteers v ON v.id = p.volunteer_id AND v.is_deleted = false
        JOIN species s ON s.id = p.species_id
        JOIN breeds b ON b.id = p.breed_id
        """;

    private const string SelectFields = """
        p.id AS Id,
        p.name AS Name,
        p.species_id AS SpeciesId,
        s.value AS SpeciesName,
        p.breed_id AS BreedId,
        b.value AS BreedName,
        p.description AS Description,
        p.color AS Color,
        p.health_info_description AS HealthInfoDescription,
        p.country AS Country,
        p.city AS City,
        p.street AS Street,
        p.house_number AS HouseNumber,
        p.weight AS Weight,
        p.height AS Height,
        p.owner_phone_number AS OwnerPhoneNumber,
        p.is_neutered AS IsNeutered,
        p.birth_date AS BirthDate,
        DATE_PART('year', AGE(p.birth_date))::int AS Age,
        p.is_vaccinated AS IsVaccinated,
        p.help_status AS HelpStatus,
        p.created_at AS CreatedAt,
        p.volunteer_id AS VolunteerId,
        v.first_name AS VolunteerFirstName,
        v.last_name AS VolunteerLastName
        """;

    public async Task<PagedResult<PetDto>> GetAll(
        GetPetsRequest request,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        var filters = new List<string> { "p.is_deleted = false" };
        var filterValues = new Dictionary<string, object?>();

        AddFilters(request, filters, filterValues);

        var whereClause = string.Join(" AND ", filters);

        var countParameters = new DynamicParameters();
        foreach (var filter in filterValues)
            countParameters.Add(filter.Key, filter.Value);

        var countSql = $"SELECT COUNT(*)::int {FromClause} WHERE {whereClause}";

        var totalCount = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(countSql, countParameters, cancellationToken: cancellationToken));

        var queryParameters = new DynamicParameters();
        foreach (var filter in filterValues)
            queryParameters.Add(filter.Key, filter.Value);

        queryParameters.Add("PageSize", request.PageSize);
        queryParameters.Add("Offset", (request.Page - 1) * request.PageSize);

        var sortColumn = GetSortColumn(request.SortBy);
        var sortDirection = string.Equals(request.SortDirection, "Asc", StringComparison.OrdinalIgnoreCase)
            ? "ASC"
            : "DESC";

        var sql = $"""
            SELECT {SelectFields}
            {FromClause}
            WHERE {whereClause}
            ORDER BY {sortColumn} {sortDirection}, p.id
            LIMIT @PageSize OFFSET @Offset
            """;

        var items = (await connection.QueryAsync<PetDto>(
            new CommandDefinition(sql, queryParameters, cancellationToken: cancellationToken))).ToList();

        await FillPhotos(items, cancellationToken);

        return new PagedResult<PetDto>(items, request.Page, request.PageSize, totalCount);
    }

    public async Task<PetDetailsDto?> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        var sql = $"""
            SELECT {SelectFields}
            {FromClause}
            WHERE p.is_deleted = false AND p.id = @Id
            """;

        var pet = await connection.QuerySingleOrDefaultAsync<PetDetailsDto>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        if (pet is null)
            return null;

        await FillPhotos([pet], cancellationToken);

        const string donationDetailsSql = """
            SELECT name AS Name, description AS Description
            FROM pet_donation_details
            WHERE pet_id = @Id
            ORDER BY id
            """;

        var donationDetails = await connection.QueryAsync<DonationDetailsRow>(
            new CommandDefinition(donationDetailsSql, new { Id = id }, cancellationToken: cancellationToken));

        pet.DonationDetails = donationDetails
            .Select(details => new DonationsRequisites(details.Name, details.Description))
            .ToList();

        return pet;
    }

    private static void AddFilters(
        GetPetsRequest request,
        ICollection<string> filters,
        IDictionary<string, object?> filterValues)
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            filters.Add("p.name ILIKE @Name");
            filterValues["Name"] = $"%{request.Name.Trim()}%";
        }

        if (!string.IsNullOrWhiteSpace(request.Species))
        {
            filters.Add("s.value ILIKE @Species");
            filterValues["Species"] = $"%{request.Species.Trim()}%";
        }

        if (!string.IsNullOrWhiteSpace(request.Breed))
        {
            filters.Add("b.value ILIKE @Breed");
            filterValues["Breed"] = $"%{request.Breed.Trim()}%";
        }

        if (!string.IsNullOrWhiteSpace(request.Color))
        {
            filters.Add("p.color ILIKE @Color");
            filterValues["Color"] = $"%{request.Color.Trim()}%";
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            filters.Add("p.city ILIKE @City");
            filterValues["City"] = $"%{request.City.Trim()}%";
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            filters.Add("p.country ILIKE @Country");
            filterValues["Country"] = $"%{request.Country.Trim()}%";
        }

        if (request.AgeFrom.HasValue)
        {
            filters.Add("DATE_PART('year', AGE(p.birth_date)) >= @AgeFrom");
            filterValues["AgeFrom"] = request.AgeFrom.Value;
        }

        if (request.AgeTo.HasValue)
        {
            filters.Add("DATE_PART('year', AGE(p.birth_date)) <= @AgeTo");
            filterValues["AgeTo"] = request.AgeTo.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.HelpStatus)
            && Enum.TryParse<HelpStatus>(request.HelpStatus, true, out var helpStatus))
        {
            filters.Add("p.help_status = @HelpStatus");
            filterValues["HelpStatus"] = helpStatus.ToString();
        }

        if (request.VolunteerId.HasValue)
        {
            filters.Add("p.volunteer_id = @VolunteerId");
            filterValues["VolunteerId"] = request.VolunteerId.Value;
        }
    }

    private static string GetSortColumn(string? sortBy)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "name" => "p.name",
            "age" => "DATE_PART('year', AGE(p.birth_date))",
            "breed" => "b.value",
            "color" => "p.color",
            "species" => "s.value",
            "city" => "p.city",
            "country" => "p.country",
            "volunteer" => "v.last_name",
            _ => "p.created_at"
        };
    }

    private async Task FillPhotos(
        IReadOnlyCollection<PetDto> pets,
        CancellationToken cancellationToken)
    {
        if (pets.Count == 0)
            return;

        await using var connection = connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);

        var petIds = pets.Select(pet => pet.Id).ToArray();

        const string sql = """
            SELECT pet_id AS PetId, path AS Path, is_main AS IsMain
            FROM pet_photos
            WHERE pet_id = ANY(@PetIds)
            ORDER BY is_main DESC, id
            """;

        var rows = await connection.QueryAsync<PetPhotoRow>(
            new CommandDefinition(sql, new { PetIds = petIds }, cancellationToken: cancellationToken));

        var photosByPet = rows
            .GroupBy(row => row.PetId)
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (var pet in pets)
        {
            if (!photosByPet.TryGetValue(pet.Id, out var photoRows))
                continue;

            var photos = new List<PetPhotoDto>(photoRows.Count);

            foreach (var row in photoRows)
            {
                var urlResult = await fileStorage.GetFileUrl(
                    FileStorageBuckets.Pets,
                    row.Path,
                    cancellationToken);

                if (urlResult.IsFailure)
                    logger.LogWarning("Failed to build URL for pet photo {PhotoPath}", row.Path);

                photos.Add(new PetPhotoDto
                {
                    Path = row.Path,
                    IsMain = row.IsMain,
                    Url = urlResult.IsSuccess ? urlResult.Value : null
                });
            }

            pet.Photos = photos;
        }
    }

    private sealed class PetPhotoRow
    {
        public Guid PetId { get; set; }

        public string Path { get; set; } = null!;

        public bool IsMain { get; set; }
    }

    private sealed class DonationDetailsRow
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;
    }
}
