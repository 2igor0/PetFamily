using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.VolunteerManagement.ValueObjects;

public record SocialNetwork
{
    public string Name { get; }

    public string Url { get; }

    private SocialNetwork()
    {
        Name = null!;
        Url = null!;
    }

    private SocialNetwork(string name, string url)
    {
        Name = name;
        Url = url;
    }

    public static Result<SocialNetwork, Error> Create(string? name, string? url)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("social.network.name.is.invalid", "Social network name cannot be empty.");

        if (string.IsNullOrWhiteSpace(url))
            return Error.Validation("social.network.url.is.invalid", "Social network URL cannot be empty.");

        var trimmedName = name.Trim();
        var trimmedUrl = url.Trim();

        if (trimmedName.Length > Constants.MaxLowTextLength)
            return Error.Validation(
                "social.network.name.is.too.long",
                $"Social network name cannot exceed {Constants.MaxLowTextLength} characters.");

        if (trimmedUrl.Length > Constants.MaxUrlLength)
            return Error.Validation(
                "social.network.url.is.too.long",
                $"Social network URL cannot exceed {Constants.MaxUrlLength} characters.");

        if (!Uri.TryCreate(trimmedUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return Error.Validation(
                "social.network.url.is.invalid",
                "Social network URL must be a valid absolute http/https URL.");
        }

        return new SocialNetwork(trimmedName, trimmedUrl);
    }
}
