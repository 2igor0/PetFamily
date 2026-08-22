using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Shared;

public record Title
{
    public string Value { get; }

    private Title(string value)
    {
        Value = value;
    }

    public static Result<Title, Error> Create(string title) //
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Error.Validation("value.is.invalid", "Title is Null Or White Space");
        }// есть кучу имплиситов, потому можем сократить return Result.Failure<Title, Error>(Error.Validation("value.is.invalid", "Title is Null Or White Space"))

        if (title.Length > Constants.MaxLowTextLength)
        {
            return Error.Validation("value.is.invalid", "Title is too long"); //1.5 45:00 доделать, можно сделать чтобы круто создавались ошибки
            
        }
        
        return new Title(title);
    }
}  