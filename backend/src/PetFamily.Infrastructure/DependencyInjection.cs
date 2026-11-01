using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using PetFamily.Application;
using PetFamily.Application.FileStorage;
using PetFamily.Infrastructure.FileStorage;
using PetFamily.Infrastructure.Read;
using PetFamily.Infrastructure.Repositories;

namespace PetFamily.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "Connection string 'Database' is not configured. Use user-secrets for local development.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IVolunteerRepository, VolunteersRepository>();

        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
        services.AddScoped<IVolunteersReadRepository, VolunteersReadRepository>();
        services.AddScoped<IPetsReadRepository, PetsReadRepository>();

        services.Configure<MinioOptions>(configuration.GetSection(MinioOptions.SectionName));

        services.AddSingleton<IMinioClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<MinioOptions>>().Value;

            var clientBuilder = new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey);

            if (options.UseSsl)
                clientBuilder = clientBuilder.WithSSL();

            return clientBuilder.Build();
        });

        services.AddSingleton<IFileStorage, MinioProvider>();

        return services;
    }
}
