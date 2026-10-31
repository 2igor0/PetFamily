using PetFamily.Domain.PetManagement.ValueObjects;
using PetFamily.Domain.Shared.DifferentId;

namespace PetFamily.UnitTests;

public class SpeciesBreedTests
{
    [Fact]
    public void Create_ValidIds_ReturnsSuccess()
    {
        var result = SpeciesBreed.Create(IdSpecies.CreateNew(), IdBreed.CreateNew());

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_EmptySpeciesId_ReturnsFailure()
    {
        var result = SpeciesBreed.Create(IdSpecies.Empty(), IdBreed.CreateNew());

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Create_EmptyBreedId_ReturnsFailure()
    {
        var result = SpeciesBreed.Create(IdSpecies.CreateNew(), IdBreed.Empty());

        Assert.True(result.IsFailure);
    }
}
