using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.modules.CreateVolunteer;
using PetFamily.Application.modules.DeleteVolunteer;
using PetFamily.Application.modules.UpdateVolunteerDonationDetails;
using PetFamily.Application.modules.UpdateVolunteerMainInfo;
using PetFamily.Application.modules.UpdateVolunteerSocialNetworks;

namespace PetFamily.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateVolunteerValidator>();

        services.AddScoped<CreateVolunteerHandler>();
        services.AddScoped<UpdateVolunteerMainInfoHandler>();
        services.AddScoped<UpdateVolunteerSocialNetworksHandler>();
        services.AddScoped<UpdateVolunteerDonationDetailsHandler>();
        services.AddScoped<DeleteVolunteerHandler>();

        return services;
    }
}
