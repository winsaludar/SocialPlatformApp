namespace Space.Domain.Exceptions;

public class InvalidSpaceCreatorException : BadRequestException
{
    public InvalidSpaceCreatorException(string name)
        : base($"Invalid space creator '{name}'") { }
}
