namespace Space.Domain.Exceptions;

public class SpaceNameAlreadyExistException : BadRequestException
{
    public SpaceNameAlreadyExistException(string name)
        : base($"The space name '{name}' is already exist") { }
}
