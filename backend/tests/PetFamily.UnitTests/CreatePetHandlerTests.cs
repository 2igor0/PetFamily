using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using PetFamily.Application;
using PetFamily.Application.FileStorage;
using PetFamily.Application.modules.CreatePet;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;
using PetFamily.Domain.VolunteerManagement;

namespace PetFamily.UnitTests;

public class CreatePetHandlerTests
{
    private readonly IVolunteerRepository _repository = Substitute.For<IVolunteerRepository>();
    private readonly IFileStorage _fileStorage = Substitute.For<IFileStorage>();

    [Fact]
    public async Task Execute_ValidRequest_CreatesPetAndUploadsPhotos()
    {
        var volunteer = TestData.CreateVolunteer();
        ConfigureVolunteer(volunteer);
        ConfigureUploadSuccess();

        var handler = CreateHandler();
        var request = CreateRequest(CreatePhoto("photo.jpg"));

        var result = await handler.Execute(volunteer.Id.Value, request);

        Assert.True(result.IsSuccess);
        Assert.Single(volunteer.Pets);

        var pet = volunteer.Pets[0];
        Assert.Single(pet.Photos);
        Assert.True(pet.Photos[0].IsMain);
        Assert.StartsWith($"{pet.Id.Value}/", pet.Photos[0].Path);

        await _repository.Received(1).SaveChanges(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_VolunteerNotFound_ReturnsNotFound()
    {
        _repository
            .GetById(Arg.Any<IdVolunteer>(), true, Arg.Any<CancellationToken>())
            .Returns((Volunteer?)null);

        var handler = CreateHandler();

        var result = await handler.Execute(Guid.NewGuid(), CreateRequest());

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        await _repository.DidNotReceive().SaveChanges(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_UploadFails_ReturnsErrorAndDoesNotSave()
    {
        var volunteer = TestData.CreateVolunteer();
        ConfigureVolunteer(volunteer);

        _fileStorage
            .UploadFile(
                Arg.Any<Stream>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Result.Failure<string, Error>(
                Error.Failure("file.upload.failed", "Failed to upload file.")));

        var handler = CreateHandler();

        var result = await handler.Execute(volunteer.Id.Value, CreateRequest(CreatePhoto("photo.jpg")));

        Assert.True(result.IsFailure);
        Assert.Empty(volunteer.Pets);
        await _repository.DidNotReceive().SaveChanges(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_SaveFails_CleansUploadedFiles()
    {
        var volunteer = TestData.CreateVolunteer();
        ConfigureVolunteer(volunteer);
        ConfigureUploadSuccess();

        _repository
            .SaveChanges(Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new InvalidOperationException("database error")));

        _fileStorage
            .DeleteFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(UnitResult.Success<Error>());

        var handler = CreateHandler();

        var result = await handler.Execute(volunteer.Id.Value, CreateRequest(CreatePhoto("photo.jpg")));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Failure, result.Error.Type);

        await _fileStorage.Received(1).DeleteFile(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_InvalidHelpStatus_ReturnsValidationError()
    {
        var volunteer = TestData.CreateVolunteer();
        ConfigureVolunteer(volunteer);

        var handler = CreateHandler();
        var request = CreateRequest() with { HelpStatus = "Unknown" };

        var result = await handler.Execute(volunteer.Id.Value, request);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        await _repository.DidNotReceive().SaveChanges(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_TooManyPhotos_ReturnsValidationError()
    {
        var volunteer = TestData.CreateVolunteer();
        ConfigureVolunteer(volunteer);

        var photos = Enumerable.Range(0, 11)
            .Select(index => CreatePhoto($"photo-{index}.jpg"))
            .ToArray();

        var handler = CreateHandler();

        var result = await handler.Execute(volunteer.Id.Value, CreateRequest(photos));

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        await _repository.DidNotReceive().SaveChanges(Arg.Any<CancellationToken>());
    }

    private CreatePetHandler CreateHandler()
    {
        return new CreatePetHandler(
            _repository,
            _fileStorage,
            NullLogger<CreatePetHandler>.Instance);
    }

    private void ConfigureVolunteer(Volunteer volunteer)
    {
        _repository
            .GetById(Arg.Any<IdVolunteer>(), true, Arg.Any<CancellationToken>())
            .Returns(volunteer);
    }

    private void ConfigureUploadSuccess()
    {
        _fileStorage
            .UploadFile(
                Arg.Any<Stream>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(call => Result.Success<string, Error>(call.ArgAt<string>(2)));
    }

    private static CreatePetRequest CreateRequest(params UploadFileDto[] photos)
    {
        return new CreatePetRequest(
            "Барсик",
            TestData.DogSpeciesId,
            TestData.LabradorBreedId,
            "Добрый и ласковый питомец",
            "Рыжий",
            "Здоров",
            "Россия",
            "Москва",
            "Ленина",
            10,
            5,
            40,
            TestData.ValidPhone,
            false,
            new DateOnly(2020, 1, 1),
            true,
            "LookingForHome",
            null,
            photos);
    }

    private static UploadFileDto CreatePhoto(string fileName)
    {
        return new UploadFileDto(
            new MemoryStream([1, 2, 3]),
            fileName,
            "image/jpeg",
            3);
    }
}
