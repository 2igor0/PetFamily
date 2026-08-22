using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagement.Enums;
using PetFamily.Domain.PetManagement.SpeciesAndBreed;
using PetFamily.Domain.PetManagement.ValueObjects;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DifferentId;


//Во время валидации использовать константы. Эти константы должны лежать в доменных моделях,
// они должны быть публичными, их можно использовать во всем приложении в
// конфигурации ef или в fluentvalidators
// ??

namespace PetFamily.Domain.PetManagement; 

public class Pet : Shared.Entity<IdPet> // Для обновления модели должны быть отдельные методы.
{
    // pet является частью внутри агригата волонтера, сделать рефакторинг
    //ef core
    private Pet() : base(IdPet.Empty()) 
    {
    }

    private Pet(Title name, Description description, IdPet id) : base(id)
    {
        Name = name;
        Description = description;
    }
    
    public Title? Name { get; private set; }
    
    public SpeciesBreed SpeciesBreed { get; private set; } // сконфигурировать
    
    public Description Description { get; private set; } // описание
    
    
    public Color Color { get; private set; }
    
    public Description HealthInfoDescription { get; private set; }

    public Address CurrentAddress { get; private set; }

    public PhysicalAttributes Weight { get; private set; }

    public PhysicalAttributes Height { get; private set; }

    public PhoneNumber OwnerPhoneNumber { get; private set; }

    public bool IsNeutered { get; private set; }  // кастрирован или нет
    
    public bool IsVaccinated { get; private set; } 

    public DateTime BirthDate { get; private set; } 
    
    public HelpStatus HelpStatus { get; private set; } // HasConversion<string>() разобраться
    
    public IReadOnlyList<DonationDetails> DonationDetails { get; private set; }
    
    public DateTime CreatedAt {get; private set;}
    
    private List<PetPhoto> _photos = []; 
    
    public IReadOnlyList<PetPhoto> Photos => _photos;
    

    public static Result<Pet, Error> Create(Title? name, Description? description, IdPet id) // какие то проверки еще на валидацию все таки из тайтле и дескриптион и еще много параметров 
    {
        if (name is null || description is null)
        {
            return Error.Validation("name.or.description.is.invalid", "name or description is invalid");
        }
        
        var pet = new Pet(name, description, id);
        
        return pet;
    }

    public Result AddPhoto(PetPhoto? photo) //Result
    {
        if (photo is null)
        {
            return Result.Failure("Photo is invalid");
        }
        
        _photos.Add(photo);
        
        return Result.Success();
    }
}