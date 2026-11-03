using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using PetFamily.Application;
using PetFamily.Application.Caching;
using PetFamily.Application.FileStorage;
using PetFamily.Infrastructure.Caching;
using PetFamily.Infrastructure.FileStorage;
using PetFamily.Infrastructure.Read;
using PetFamily.Infrastructure.Repositories;
using StackExchange.Redis;

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

        services.AddScoped<VolunteersReadRepository>();
        services.AddScoped<IVolunteersReadRepository>(serviceProvider =>
            new CachedVolunteersReadRepository(
                serviceProvider.GetRequiredService<VolunteersReadRepository>(),
                serviceProvider.GetRequiredService<ICacheService>()));

        services.AddScoped<PetsReadRepository>();
        services.AddScoped<IPetsReadRepository>(serviceProvider =>
            new CachedPetsReadRepository(
                serviceProvider.GetRequiredService<PetsReadRepository>(),
                serviceProvider.GetRequiredService<ICacheService>()));

        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddSingleton<ICacheService, NoOpCacheService>();
        }
        else
        {
            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var options = ConfigurationOptions.Parse(redisConnectionString);
                options.AbortOnConnectFail = false;

                return ConnectionMultiplexer.Connect(options);
            });

            services.AddSingleton<ICacheService, RedisCacheService>();
        }

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
