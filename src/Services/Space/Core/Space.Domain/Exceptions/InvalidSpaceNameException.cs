namespace Space.Domain.Exceptions;

public class InvalidSpaceNameException : BadRequestException
{
    public InvalidSpaceNameException(string name)
        : base($"Invalid space name '{name}'") { }
}
