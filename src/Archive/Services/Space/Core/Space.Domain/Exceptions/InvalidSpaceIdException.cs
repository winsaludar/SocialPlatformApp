namespace Space.Domain.Exceptions;

public class InvalidSpaceIdException : BadRequestException
{
    public InvalidSpaceIdException(Guid id)
        : base($"Invalid space id '{id}'") { }
}
