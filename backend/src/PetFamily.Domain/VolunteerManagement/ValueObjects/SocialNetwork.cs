using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.VolunteerManagement.ValueObjects;

public record SocialNetwork
{
    public string Name { get; } 
    
    public string Url { get; }
    
    private  SocialNetwork(string name, string url)
    {
        Name = name;
        Url = url;
    }

    public static Result<SocialNetwork, Error> Create(string name, string url)
    {
        //валидация
        return new SocialNetwork(name, url);
    }
}